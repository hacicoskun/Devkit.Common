# Devkit.Common 🚀  
**Uygulama altyapılarını sadeleştiren .NET kütüphanesi**

**Devkit.Common**, .NET projelerinde tekrarlayan altyapı ihtiyaçlarını kolayca yönetmek için geliştirilmiş bir kütüphanedir.  
Amaç, farklı projelerde **ortak çözümleri** yeniden kullanılabilir hale getirerek, daha **tutarlı** ve **bakımı kolay** sistemler kurmaktır.

---

## ⚙️ Şu Anda Neler Var?
- **Outbox & Inbox Pattern** desteğiyle güvenilir mesajlaşma  
- **RabbitMQ entegrasyonu**  
- **Çoklu broker desteği** (Kafka ve diğerleri için hazır yapı)  

---

## 🔜 Planlanan Özellikler
- **Hangfire** entegrasyonu  
- **Job Scheduler / Background Task** desteği  
- **Logging & Telemetri** bileşenleri  
- **NuGet paket yayını**

---

## 🧩 Hızlı Başlangıç

```csharp
builder.Services.AddMessaging<AppDbContext>(
    builder.Configuration,
    consumerAssembly: typeof(Program).Assembly,
    enableConsumers: true,
    enableOutbox: true
); 
