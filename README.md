# Yazılım Öğrenme Günlüğü

Mades Kulübü Yazılım Öğrenme Rehberliği Etkinliği Takip Sistemi

## Proje Hakkında
26 Ocak - 16 Şubat 2026 tarihleri arasında gerçekleşecek yazılım öğrenme etkinliği için katılımcı takip platformu.

## Teknolojiler
- Frontend: HTML, CSS, JavaScript (Vanilla JS)
- Backend: C# (ASP.NET Core Web API)
- Database: SQLite

## Özellikler

### Kullanıcı Sistemi
- Ad ve soyad ile kayıt/giriş
- İki kullanıcı tipi: Mentör ve Katılımcı
- Mentörler: Cihangir Yaman, İbrahim Kabadayı, Eren Vural

### Katılımcı Özellikleri
- Günlük gelişim formu (5 soru)
- GitHub-tarzı ilerleme grafiği
- Haftalık gelişim kartları
- Mentör geri bildirimleri
- Bildirim sistemi
- Soru-cevap bölümü

### Mentör Özellikleri
- Tüm katılımcıların raporlarını görüntüleme
- "Görüldü" ve "Tebrikler" geri bildirimi verme
- Tüm katılımcıların ilerleme grafiklerini görüntüleme
- Soru-cevap bölümü

## Kurulum

### Backend Kurulumu

1. Backend dizinine gidin:
```bash
cd Backend
```

2. Gerekli paketleri yükleyin:
```bash
dotnet restore
```

3. Uygulamayı çalıştırın:
```bash
dotnet run
```

Backend http://localhost:5000 adresinde çalışacaktır.

### Frontend Kurulumu

1. Frontend dizinine gidin:
```bash
cd Frontend
```

2. Basit bir HTTP sunucusu ile çalıştırın. Python 3 ile:
```bash
python -m http.server 8080
```

Veya Node.js ile http-server kullanarak:
```bash
npx http-server -p 8080
```

Frontend http://localhost:8080 adresinde erişilebilir olacaktır.

## Kullanım

1. Tarayıcınızda http://localhost:8080 adresine gidin
2. Kayıt olun veya giriş yapın
3. Mentör kullanıcıları için özel isimler: Cihangir Yaman, İbrahim Kabadayı, Eren Vural
4. Katılımcılar günlük formlarını doldurabilir ve ilerlemelerini takip edebilir
5. Mentörler tüm katılımcıların raporlarını görebilir ve geri bildirim verebilir

## Proje Yapısı

```
/
├── Backend/
│   ├── Controllers/      # API Controllers
│   ├── Models/          # Database Models
│   ├── DTOs/            # Data Transfer Objects
│   ├── Services/        # Business Logic
│   ├── Data/            # Database Context
│   └── Program.cs       # Main Entry Point
├── Frontend/
│   ├── index.html       # Main HTML
│   ├── styles/
│   │   └── main.css     # Styling
│   ├── scripts/
│   │   └── app.js       # JavaScript Logic
│   └── assets/
├── README.md
└── .gitignore
```

## Etkinlik Tarihleri
- Etkinlik başlangıcı: 26 Ocak 2026
- Etkinlik bitişi: 16 Şubat 2026
- İlk 2 hafta: C# öğretimi
- Son hafta: Git & GitHub öğretimi

## Lisans
Bu proje Mades Kulübü için geliştirilmiştir.

