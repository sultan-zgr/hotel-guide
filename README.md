# Otel ve Rapor Yönetimi Mikroservis Projesi

Bu proje, otel ve rapor yönetimini sağlamak amacıyla tasarlanmış bir **mikroservis mimarisi** uygulamasıdır. Proje **.NET 8** framework'ü kullanılarak geliştirilmiş ve **RabbitMQ**, **PostgreSQL**, **FluentValidation**, **AutoMapper** gibi modern teknolojilerle desteklenmiştir. **API Gateway (Ocelot)** ile merkezi bir giriş noktası sunar.

## **Temel Amaçlar**
- **Otel Yönetimi:** Otel ve iletişim bilgilerinin merkezi bir sistem üzerinden yönetimi.
- **Rapor Yönetimi:** Konum bazlı rapor taleplerinin asenkron olarak işlenmesi.
- **Mesajlaşma Entegrasyonu:** Mikroservislerin RabbitMQ kullanarak birbiriyle iletişim kurması.
- **Event Bazlı İletişim:** Servislerin birbirleriyle olay bazlı (event-driven) mesajlaşma yoluyla senkronize edilmesi.
- **Geliştirilebilir Altyapı:** Elasticsearch gibi ek özelliklerin kolayca entegre edilebilmesi.

---

## **Kullanılan Mimari**

### **Mikroservis Mimarisi**
Bu proje, her bir işlevselliğin bağımsız bir servis tarafından yönetildiği mikroservis mimarisini temel alır. Bu mimari sayesinde:
- Her bir servis bağımsız olarak geliştirilebilir, dağıtılabilir ve ölçeklenebilir.
- RabbitMQ kullanılarak servisler arasında asenkron iletişim sağlanır.
- API Gateway ile merkezi bir giriş noktası oluşturularak isteklerin doğru servise yönlendirilmesi sağlanır.

### **Event-Driven Architecture (Olay Tabanlı Mimari)**
- **Event Bazlı Mesajlaşma:** RabbitMQ ile event-driven bir yapı kurulmuştur.
  - **HotelService:** Yeni otel ekleme, güncelleme ve silme işlemlerini olaylar (event) olarak yayınlar.
  - **ReportService:** HotelService tarafından yayınlanan otel olaylarını dinleyerek kendi veritabanını senkronize eder.
- Bu yapı, servislerin birbirinden bağımsız çalışmasını ve asenkron bir iletişim sağlanmasını mümkün kılar.

### **Arka Planda Çalışan Servis (Background Worker)**

**Rapor Durumunu Güncelleyen Arka Plan Servisi**
- **Worker:** ReportService için bir arka plan servisi olarak yapılandırılmıştır.
- **Amaç:** Kullanıcının talep ettiği raporların arka planda hazırlanmasını ve tamamlandığında durumunun "Preparing" (Hazırlanıyor) olarak güncellenip "Completed" (Tamamlandı) durumuna geçirilmesini sağlar.
- **Çalışma Yapısı:**
  1. **RabbitMQ Dinleme:** Worker, `report-queue` kuyruğunu dinler.
  2. **Durum Güncelleme:** Talep edilen raporlar kuyruktan alınır ve ilgili raporun durum bilgisi güncellenir.
  3. **Database ile Senkronizasyon:** Durum değişikliği sonrası değişiklikler PostgreSQL veritabanına kaydedilir.
  
**Bu yapı sayesinde kullanıcı talepleri senkronize edilmeden işlenir, böylece sistem darboğazlara karşı korunur.**

---

## **Servisler**
- **HotelService:** Otel ve iletişim bilgilerini yönetmek için kullanılır.
- **ReportService:** Konum bazlı raporların hazırlanmasını ve işlenmesini sağlar.

### **Veri Tabanı**
- PostgreSQL kullanılarak veri depolama işlemleri gerçekleştirilmiştir.
- Her servis kendi veritabanına sahiptir (Database per Service).

### **Mesajlaşma Sistemi**
- RabbitMQ, servisler arasında olay tabanlı mesajlaşma için kullanılmıştır.

### **API Gateway**
- Merkezi bir giriş noktası olarak yapılandırılmıştır.
- Ratelimiting gibi özelliklerle güvenlik ve trafik kontrolü sağlar.

---

## 🚀 Özellikler

- **Otel Yönetimi:**
  - CRUD işlemleri (Ekle, Güncelle, Listele, Sil).
  - İletişim bilgileri yönetimi.
  - **Serilog ile Loglama**: Tüm otel işlemleri loglanır.
- **Rapor Yönetimi:**
  - Otel raporlarının hazırlanması ve kuyruk mekanizmasıyla asenkron işlenmesi.
  - **Serilog ile Loglama**: Rapor talepleri ve durum güncellemeleri loglanır.
- **RabbitMQ Entegrasyonu:** Mikroservisler arasında mesajlaşma.
- **PostgreSQL:** Veri depolama.
- **FluentValidation:** DTO seviyesinde validasyon.
- **Unit Testler:** HotelService ve ContactService için **xUnit** testleri.
- **AutoMapper:** Veri transfer objeleri ve modeller arasında otomatik eşleme.
- **API Gateway:** **Ocelot** kullanılarak tüm API istekleri merkezi bir noktadan yönetilir.
- **Docker Desteği:** Tüm servislerin kolayca ayağa kaldırılması. 

---

## 🛠️ Kullanılan Teknolojiler

- **.NET 8**
- **PostgreSQL**
- **RabbitMQ**
- **Ocelot (API Gateway)**
- **FluentValidation**
- **AutoMapper**
- **Serilog**
- **Docker**
- **xUnit**

---

## 📦 Kurulum ve Çalıştırma

### 1️⃣ Gereksinimler
Projeyi çalıştırmadan önce aşağıdaki yazılımların sisteminizde kurulu olması gerekmektedir:
- **Docker**
- **.NET SDK (8.0+)**

### 2️⃣ Docker İle Servisleri Ayağa Kaldırma
Projede tüm mikroservisler, PostgreSQL veritabanları, RabbitMQ ve API Gateway **Docker Compose** kullanılarak kolayca ayağa kaldırılabilir:

```bash
docker-compose up --build
```
Tüm servisler, RabbitMQ (port: 5672, 15672) ve PostgreSQL (port: 5432, 5433) ile birlikte ayağa kalkacaktır.

### 3️⃣ Migration İşlemleri
Veritabanı tablolarını oluşturmak için aşağıdaki adımları takip edin:

HotelService için:
```bash
cd HotelService
dotnet ef migrations add InitialMigration
dotnet ef database update
```
HotelService için:
```bash
cd ReportService
dotnet ef migrations add InitialMigration
dotnet ef database update
```
## 📚 Servisler ve Endpointler

### HotelService
- **GET** `/api/hotels`  
  Tüm otelleri listele.

- **POST** `/api/hotels`  
  Yeni otel ekle.

- **GET** `/api/hotels/bulk`  
  Toplu otel ekle.

- **PUT** `/api/hotels/{id}`  
  Mevcut oteli güncelle.

- **DELETE** `/api/hotels/{id}`  
  Oteli sil.

- **GET** `/api/hotels/{id}`  
  Belirli bir otel bilgisi getir.


---

### ReportService
- **GET** `/api/reports`  
  Tüm raporları listele.

- **POST** `/api/reports`  
  Yeni rapor talebi oluştur.

- **GET** `/api/reports/{id}`  
  Rapor detaylarını getir.

---

## 🧪 Projeyi Test Etme

Projede unit testler **xUnit** kullanılarak yazılmıştır. Testleri çalıştırmak için şu komutları kullanabilirsiniz:

```bash
# HotelService Testlerini Çalıştır
cd HotelService.Tests
dotnet test

# ReportService Testlerini Çalıştır
cd ReportService.Tests
dotnet test
```
<sub>🔮 **Not:** Elasticsearch Entegrasyonu ile arama ve raporlama için güçlü bir altyapı planlanmaktadır.</sub>

