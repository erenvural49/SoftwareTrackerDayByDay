// API Configuration
const API_BASE_URL = "http://localhost:5277/api";

// State Management
let currentUser = null;
let authToken = null;

// Initialize App
document.addEventListener('DOMContentLoaded', () => {
    initAuth();
    setupEventListeners();
});

// Auth Functions
function initAuth() {
    const savedToken = localStorage.getItem('authToken');
    const savedUser = localStorage.getItem('currentUser');
    
    if (savedToken && savedUser) {
        authToken = savedToken;
        currentUser = JSON.parse(savedUser);
        showDashboard();
    } else {
        showAuthPage();
    }
}

function showAuthPage() {
    document.getElementById('auth-page').classList.add('active');
    document.getElementById('dashboard-page').classList.remove('active');
}

function showDashboard() {
    document.getElementById('auth-page').classList.remove('active');
    document.getElementById('dashboard-page').classList.add('active');
    
    document.getElementById('user-name').textContent = `${currentUser.firstName} ${currentUser.lastName} (${currentUser.role === 'Mentor' ? 'Mentör' : 'Katılımcı'})`;
    
    // Hide/Show tabs based on role
    if (currentUser.role === 'Mentor') {
        document.getElementById('daily-form-tab').style.display = 'none';
        document.getElementById('reports-tab').textContent = 'Tüm Raporlar';
        document.getElementById('mentor-participants-section').style.display = 'block';
    } else {
        document.getElementById('daily-form-tab').style.display = 'block';
        document.getElementById('reports-tab').textContent = 'Raporlarım';
        document.getElementById('mentor-participants-section').style.display = 'none';
    }
    
    loadDashboardData();
}

function setupEventListeners() {
    // Auth tabs
    document.querySelectorAll('.tab-btn').forEach(btn => {
        btn.addEventListener('click', (e) => {
            const tab = e.target.dataset.tab;
            document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
            document.querySelectorAll('.auth-form').forEach(f => f.classList.remove('active'));
            e.target.classList.add('active');
            document.getElementById(`${tab}-form`).classList.add('active');
        });
    });
    
    // Login form
    document.getElementById('loginForm').addEventListener('submit', async (e) => {
        e.preventDefault();
        const firstName = document.getElementById('login-firstname').value;
        const lastName = document.getElementById('login-lastname').value;
        await login(firstName, lastName);
    });
    
    // Register form
    document.getElementById('registerForm').addEventListener('submit', async (e) => {
        e.preventDefault();
        const firstName = document.getElementById('register-firstname').value;
        const lastName = document.getElementById('register-lastname').value;
        await register(firstName, lastName);
    });
    
    // Logout
    document.getElementById('logout-btn').addEventListener('click', logout);
    
    // Dashboard tabs
    document.querySelectorAll('.dash-tab-btn').forEach(btn => {
        btn.addEventListener('click', (e) => {
            const tab = e.target.dataset.tab;
            document.querySelectorAll('.dash-tab-btn').forEach(b => b.classList.remove('active'));
            document.querySelectorAll('.tab-content').forEach(c => c.classList.remove('active'));
            e.target.classList.add('active');
            document.getElementById(`${tab}-tab-content`).classList.add('active');
            
            // Load data for specific tabs
            if (tab === 'reports') {
                loadReports();
            } else if (tab === 'qa') {
                loadQuestions();
            }
        });
    });
    
    // Daily report form
    document.getElementById('daily-report-form').addEventListener('submit', async (e) => {
        e.preventDefault();
        await submitDailyReport();
    });
    
    // Question form
    document.getElementById('question-form').addEventListener('submit', async (e) => {
        e.preventDefault();
        await submitQuestion();
    });
}

async function login(firstName, lastName) {
    try {
        const response = await fetch(`${API_BASE_URL}/Auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ firstName, lastName })
        });
        
        const data = await response.json();
        
        if (!response.ok) {
            showMessage('auth-message', data.message || 'Giriş başarısız', 'error');
            return;
        }
        
        authToken = data.token;
        currentUser = data;
        localStorage.setItem('authToken', authToken);
        localStorage.setItem('currentUser', JSON.stringify(currentUser));
        
        showDashboard();
    } catch (error) {
        showMessage('auth-message', 'Bağlantı hatası', 'error');
    }
}

async function register(firstName, lastName) {
    try {
        const response = await fetch(`${API_BASE_URL}/Auth/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ firstName, lastName })
        });
        
        const data = await response.json();
        
        if (!response.ok) {
            showMessage('auth-message', data.message || 'Kayıt başarısız', 'error');
            return;
        }
        
        authToken = data.token;
        currentUser = data;
        localStorage.setItem('authToken', authToken);
        localStorage.setItem('currentUser', JSON.stringify(currentUser));
        
        showDashboard();
    } catch (error) {
        showMessage('auth-message', 'Bağlantı hatası', 'error');
    }
}

function logout() {
    localStorage.removeItem('authToken');
    localStorage.removeItem('currentUser');
    authToken = null;
    currentUser = null;
    showAuthPage();
}

async function loadDashboardData() {
    await Promise.all([
        loadNotifications(),
        loadWeeklyCards(),
        loadContributions(),
        loadParticipants()
    ]);
}

async function loadNotifications() {
    if (currentUser.role === 'Mentor') return;
    
    try {
        const response = await fetch(`${API_BASE_URL}/Notifications`, {
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        
        if (!response.ok) return;
        
        const notifications = await response.json();
        const container = document.getElementById('notifications-container');
        
        if (notifications.length === 0) {
            container.innerHTML = '';
            return;
        }
        
        container.innerHTML = notifications.map(n => `
            <div class="notification ${n.isRead ? 'read' : ''}">
                <span>${n.message}</span>
                ${!n.isRead ? `<button onclick="markNotificationRead(${n.id})" class="btn-secondary">Okundu İşaretle</button>` : ''}
            </div>
        `).join('');
    } catch (error) {
        console.error('Failed to load notifications:', error);
    }
}

async function markNotificationRead(id) {
    try {
        await fetch(`${API_BASE_URL}/Notifications/${id}/mark-read`, {
            method: 'POST',
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        await loadNotifications();
    } catch (error) {
        console.error('Failed to mark notification as read:', error);
    }
}

async function loadWeeklyCards() {
    if (currentUser.role === 'Mentor') return;
    
    try {
        const response = await fetch(`${API_BASE_URL}/WeeklyCards`, {
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        
        if (!response.ok) return;
        
        const cards = await response.json();
        const container = document.getElementById('weekly-cards-container');
        
        if (cards.length === 0) {
            container.innerHTML = '';
            return;
        }
        
        container.innerHTML = cards.map(c => `
            <div class="weekly-card">
                <h3>Hafta ${c.weekNumber}</h3>
                <div class="stats">
                    <div class="stat"><strong>${c.daysWorked}</strong> gün çalıştın</div>
                    <div class="stat"><strong>${c.topicsLearned}</strong> konu öğrendin</div>
                    <div class="stat"><strong>${c.errorsSolved}</strong> hata çözdün</div>
                </div>
                <div class="message">${c.message}</div>
            </div>
        `).join('');
    } catch (error) {
        console.error('Failed to load weekly cards:', error);
    }
}

async function loadContributions(userId = null) {
    try {
        const url = userId 
            ? `${API_BASE_URL}/DailyReports/contributions/${userId}`
            : `${API_BASE_URL}/DailyReports/contributions`;
            
        const response = await fetch(url, {
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        
        if (!response.ok) return;
        
        const contributions = await response.json();
        const container = document.getElementById('contribution-graph');
        
        container.innerHTML = contributions.map(c => {
            const date = new Date(c.date);
            const dateStr = date.toLocaleDateString('tr-TR');
            return `<div class="contribution-day" data-level="${c.completionLevel}" title="${dateStr}: ${c.completionLevel}/5"></div>`;
        }).join('');
    } catch (error) {
        console.error('Failed to load contributions:', error);
    }
}

async function loadParticipants() {
    if (currentUser.role !== 'Mentor') return;
    
    try {
        const response = await fetch(`${API_BASE_URL}/DailyReports/all-reports`, {
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        
        if (!response.ok) return;
        
        const reports = await response.json();
        
        // Get unique participants
        const participants = {};
        reports.forEach(r => {
            if (!participants[r.userId]) {
                participants[r.userId] = {
                    userId: r.userId,
                    userName: r.userName,
                    reportCount: 0
                };
            }
            participants[r.userId].reportCount++;
        });
        
        const container = document.getElementById('participants-list');
        container.innerHTML = Object.values(participants).map(p => `
            <div class="participant-card" onclick="viewParticipant(${p.userId})">
                <div class="participant-name">${p.userName}</div>
                <div class="participant-stats">${p.reportCount} rapor gönderildi</div>
            </div>
        `).join('');
    } catch (error) {
        console.error('Failed to load participants:', error);
    }
}

function viewParticipant(userId) {
    loadContributions(userId);
    // Scroll to contribution graph
    document.querySelector('.contribution-section').scrollIntoView({ behavior: 'smooth' });
}

async function submitDailyReport() {
    const data = {
        whatILearned: document.getElementById('what-learned').value,
        whereIStruggled: document.getElementById('where-struggled').value,
        errorsSolved: document.getElementById('errors-solved').value,
        tomorrowGoal: document.getElementById('tomorrow-goal').value,
        effortLevel: document.querySelector('input[name="effort"]:checked')?.value
    };
    
    if (!data.effortLevel) {
        showMessage('form-message', 'Lütfen tüm alanları doldurun', 'error');
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE_URL}/DailyReports`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify(data)
        });
        
        const result = await response.json();
        
        if (!response.ok) {
            showMessage('form-message', result.message || 'Form gönderilemedi', 'error');
            return;
        }
        
        showMessage('form-message', 'Form başarıyla kaydedildi!', 'success');
        document.getElementById('daily-report-form').reset();
        
        // Reload contributions
        await loadContributions();
    } catch (error) {
        showMessage('form-message', 'Bağlantı hatası', 'error');
    }
}

async function loadReports() {
    try {
        const endpoint = currentUser.role === 'Mentor' 
            ? `${API_BASE_URL}/DailyReports/all-reports`
            : `${API_BASE_URL}/DailyReports/my-reports`;
            
        const response = await fetch(endpoint, {
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        
        if (!response.ok) return;
        
        const reports = await response.json();
        const container = document.getElementById('reports-container');
        
        if (reports.length === 0) {
            container.innerHTML = '<p>Henüz rapor bulunmuyor.</p>';
            return;
        }
        
        container.innerHTML = reports.map(r => {
            const date = new Date(r.date);
            const dateStr = date.toLocaleDateString('tr-TR');
            const effortClass = r.effortLevel === 'Güzel' ? 'good' : r.effortLevel === 'Orta' ? 'medium' : 'bad';
            
            return `
                <div class="report-card">
                    <div class="report-header">
                        <div>
                            <div class="report-date">${dateStr}</div>
                            ${currentUser.role === 'Mentor' ? `<div style="font-size: 14px; color: #57606a;">${r.userName}</div>` : ''}
                        </div>
                        <div class="report-effort ${effortClass}">${r.effortLevel}</div>
                    </div>
                    <div class="report-content">
                        <div class="report-item">
                            <strong>Bugün ne öğrendim:</strong>
                            <p>${r.whatILearned}</p>
                        </div>
                        <div class="report-item">
                            <strong>En çok nerede zorlandım:</strong>
                            <p>${r.whereIStruggled}</p>
                        </div>
                        <div class="report-item">
                            <strong>Çözdüğüm hata:</strong>
                            <p>${r.errorsSolved}</p>
                        </div>
                        <div class="report-item">
                            <strong>Yarınki hedefim:</strong>
                            <p>${r.tomorrowGoal}</p>
                        </div>
                    </div>
                    ${currentUser.role === 'Mentor' ? `
                        <div class="report-feedbacks">
                            <button class="feedback-btn ${r.feedbacks.some(f => f.feedbackType === 'Görüldü') ? 'active' : ''}" 
                                    onclick="giveFeedback(${r.id}, 'Görüldü')" 
                                    ${r.feedbacks.some(f => f.feedbackType === 'Görüldü') ? 'disabled' : ''}>
                                Görüldü
                            </button>
                            <button class="feedback-btn ${r.feedbacks.some(f => f.feedbackType === 'Tebrikler') ? 'active' : ''}" 
                                    onclick="giveFeedback(${r.id}, 'Tebrikler')"
                                    ${r.feedbacks.some(f => f.feedbackType === 'Tebrikler') ? 'disabled' : ''}>
                                Tebrikler
                            </button>
                        </div>
                    ` : r.feedbacks.length > 0 ? `
                        <div class="report-feedbacks">
                            ${r.feedbacks.map(f => `
                                <span class="feedback-tag">${f.feedbackType} - ${f.mentorName}</span>
                            `).join('')}
                        </div>
                    ` : ''}
                </div>
            `;
        }).join('');
    } catch (error) {
        console.error('Failed to load reports:', error);
    }
}

async function giveFeedback(reportId, feedbackType) {
    try {
        const response = await fetch(`${API_BASE_URL}/Feedback`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify({
                dailyReportId: reportId,
                feedbackType: feedbackType
            })
        });
        
        if (!response.ok) return;
        
        await loadReports();
    } catch (error) {
        console.error('Failed to give feedback:', error);
    }
}

async function loadQuestions() {
    try {
        const response = await fetch(`${API_BASE_URL}/Questions`, {
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        
        if (!response.ok) return;
        
        const questions = await response.json();
        const container = document.getElementById('questions-container');
        
        if (questions.length === 0) {
            container.innerHTML = '<p style="text-align: center; color: #57606a;">Henüz soru sorulmamış.</p>';
            return;
        }
        
        container.innerHTML = questions.map(q => {
            const date = new Date(q.createdAt);
            const timeStr = date.toLocaleString('tr-TR');
            
            return `
                <div class="question-item">
                    <div class="question-header">
                        <span class="question-author">${q.userName}</span>
                        <span class="question-time">${timeStr}</span>
                    </div>
                    <div class="question-content">${q.content}</div>
                </div>
            `;
        }).join('');
        
        // Scroll to bottom
        container.scrollTop = container.scrollHeight;
    } catch (error) {
        console.error('Failed to load questions:', error);
    }
}

async function submitQuestion() {
    const content = document.getElementById('question-input').value;
    
    if (!content.trim()) return;
    
    try {
        const response = await fetch(`${API_BASE_URL}/Questions`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify({ content })
        });
        
        if (!response.ok) return;
        
        document.getElementById('question-input').value = '';
        await loadQuestions();
    } catch (error) {
        console.error('Failed to submit question:', error);
    }
}

function showMessage(elementId, message, type) {
    const element = document.getElementById(elementId);
    element.textContent = message;
    element.className = `message ${type}`;
    
    setTimeout(() => {
        element.className = 'message';
    }, 5000);
}

// Make functions global for onclick handlers
window.markNotificationRead = markNotificationRead;
window.viewParticipant = viewParticipant;
window.giveFeedback = giveFeedback;
