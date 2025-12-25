# 🚀 Devkit.Common: .NET Altyapısını Sadeleştirme Gücü

**.NET projelerinde tekrarlayan altyapı ihtiyaçlarını kolayca yönetmek için geliştirilmiş kapsamlı bir kütüphane.**

Devkit.Common, .NET geliştiricilerinin sürekli karşılaştığı **tekrarlayan altyapı zorluklarını** çözmek için tasarlanmıştır. Amacımız, projelerinizde **tutarlılığı** ve **bakım kolaylığını** en üst düzeye çıkarmak için kanıtlanmış ortak çözümleri **tekrar kullanılabilir** bileşenlere dönüştürmektir.

> ✨ **Daha az boilerplate, daha fazla iş mantığı.** Projelerinizi daha hızlı, daha güvenilir ve daha yönetilebilir hale getirin.

---

## ⚙️ Mevcut Özellikler (Hemen Kullanıma Hazır!)

Bu kütüphane, dağıtık sistemler ve performans odaklı uygulamalar için kritik öneme sahip temel özellikleri içerir:

| Kategori | Özellik | Açıklama |
| :--- | :--- | :--- |
| **Mesajlaşma** | **Outbox & Inbox Pattern** | Veritabanı işlemleri ve mesaj gönderme/alma arasında tutarlılığı garanti eden sağlam destek. |
| **Mesaj Broker** | **RabbitMQ Entegrasyonu** | Endüstri standardı mesaj broker'ı ile hızlı ve kolay entegrasyon. |
| **Broker Mimarisi** | **Çoklu Broker Desteği** | İhtiyaç halinde **Kafka** gibi farklı mesaj broker'larına geçiş yapabilmeniz için esnek altyapı. |
| **Önbellekleme** | **InMemory / Redis / Hybrid Cache** | Uygulamanızın performansını artırmak için esnek önbellek çözümleri. |
| **Job** | **Hangfire / Quartz** | Uygulamanız için tekrarlayan iş yöneticisi. |
| **Identity** | **Keycloak / AspnetIdentity** | Kullanıcı ve rol yönetimi. |

---

## 🔜 Yol Haritası (Gelecek Özellikler)

Geliştirme sürecindeki heyecan verici eklemelerle kütüphanemizi güçlendirmeye devam ediyoruz:

* 📊 **Logging & Telemetri Bileşenleri:** Merkezi izleme ve analiz için kapsamlı bileşenler.
* 📦 **Resmi NuGet Paket Yayını:** Kolay kurulum ve yönetim için tüm bileşenlerin NuGet üzerinden erişilebilir hale getirilmesi.

---

## 🧩 Hızlı Başlangıç: Dakikalar İçinde Hazır!

**Devkit.Common**'ı projenize entegre etmek, standart .NET Bağımlılık Enjeksiyonu yapısını kullanarak yalnızca birkaç satır kod gerektirir.

### **1. Güvenilir Mesajlaşma ve Outbox Entegrasyonu**

Uygulamanızın `Program.cs` dosyasına aşağıdaki kodu ekleyerek güvenilir mesajlaşma altyapısını anında aktif edin:

```csharp
// Devkit.Common'ı kullanarak güvenilir mesajlaşma servisini ekler.
builder.Services.AddMessaging(
    builder.Configuration,
    // Consumer'ların hangi Assembly'de olduğunu belirtiriz.
    consumerAssembly: typeof(Program).Assembly, 
    // Consumer'ların bu serviste çalıştırılmasını sağlar.
    useConsumers: true 
);

// Devkit.Common'ı kullanarak güvenilir mesajlaşma (Outbox) servisini ekler.
builder.Services.AddMessagingWithOutbox<AppDbContext>(
    builder.Configuration,
    // Consumer'ların hangi Assembly'de olduğunu belirtiriz.
    consumerAssembly: typeof(Program).Assembly, 
    // Consumer'ların bu serviste çalıştırılmasını sağlar.
    useConsumers: true 
);

// Cache
builder.Services.AddCacheProvider(builder.Configuration); 

//Job
builder.Services.AddJobScheduler(builder.Configuration);

// Keycloak Kullanımı (Merkezi Auth Sunucusu)
// appsettings.json üzerinden "Provider": "Keycloak" ayarlanmalıdır.
builder.Services.AddDevkitIdentity(builder.Configuration, builder.Environment);

// AspNetIdentity Kullanımı (Veritabanı Tabanlı JWT)
// appsettings.json üzerinden "Provider": "AspNetIdentity" ayarlanmalıdır.
// Kendi DbContext'inizi (<AppDbContext>) generic olarak belirtmeniz gerekir.
// AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
builder.Services.AddIdentity<AppDbContext>(builder.Configuration, builder.Environment);



