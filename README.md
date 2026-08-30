# Market Automation

Windows için market / bakkal otomasyon yazılımı. Ürün yönetimi, kategori yönetimi, kasa (POS), stok takibi ve raporlama özellikleri sunar.

## Özellikler

- **Kasa (POS)** — Barkod veya ürün seçimi ile satış
- **Ürün Yönetimi** — Ürün ekleme, düzenleme, stok takibi
- **Kategori Yönetimi** — Ürün kategorileri
- **Raporlar** — Satış ve stok raporları
- **Ayarlar** — Market adı, kasiyer, iletişim bilgileri

## Sistem Gereksinimleri

- Windows 10 veya üzeri (64-bit)
- Ek kurulum gerekmez — .NET runtime dahildir (self-contained)

## Son Kullanıcı Kurulumu

1. `dist/MarketAutomation-v1.0.0-win-x64.zip` dosyasını indirin
2. Zip dosyasını istediğiniz bir klasöre çıkarın (ör. `C:\MarketAutomation`)
3. **`MarketAutomation.exe`** dosyasını çalıştırın

Launcher otomatik olarak API ve masaüstü uygulamasını başlatır. Masaüstü uygulaması kapandığında API de kapanır.

### Veri Konumları

Tüm kullanıcı verileri şu klasörde saklanır:

```
%LOCALAPPDATA%\MarketAutomation2\
├── MarketAutomation.db    (veritabanı)
├── settings.json          (uygulama ayarları)
└── Certificates\          (HTTPS sertifikası)
```

## Geliştirici Kurulumu

### Gereksinimler

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 (önerilir) veya VS Code

### Projeyi Derleme

```powershell
dotnet build MarketAutomation2.slnx -c Release
```

### Dağıtım Paketi Oluşturma

```powershell
.\scripts\publish.ps1
```

Bu script şunları oluşturur:

```
dist/
├── MarketAutomation/              # Dağıtım klasörü
│   ├── MarketAutomation.exe       # Başlatıcı (bunu çalıştırın)
│   ├── Api/                       # Arka plan API servisi
│   └── Desktop/                   # WPF masaüstü uygulaması
└── MarketAutomation-v1.0.0-win-x64.zip
```

Zip dosyasını paylaşarak son kullanıcılara ulaştırabilirsiniz.

### Geliştirme Ortamında Çalıştırma

Debug modunda Launcher, publish klasörü yoksa otomatik olarak Debug build çıktılarını kullanır:

1. API'yi başlatın veya Launcher'ı Debug modda çalıştırın
2. API adresi: `https://localhost:7116`

## Proje Yapısı

| Proje | Açıklama |
|-------|----------|
| `MarketAutomation.Launcher` | API + Desktop başlatıcı |
| `MarketAutomation2.API` | ASP.NET Core REST API (SQLite) |
| `MarketAutomation2.Desktop` | WPF masaüstü arayüzü |

## Lisans

Bu proje özel kullanım içindir.
