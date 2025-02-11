# ✈ FlightApp - Uçuş Rezervasyon Sistemi  

## 📌 Proje Hakkında  
**FlightApp**, **ASP.NET Core MVC** kullanılarak geliştirilmiş bir **uçuş rezervasyon sistemi** web uygulamasıdır. Kullanıcılar uçuş arayabilir, bilet satın alabilir, uçuş rotalarını harita üzerinde görüntüleyebilir ve hesap bilgilerini yönetebilirler. Admin kullanıcılar ise **CRUD işlemleri**, SOAP ve gRPC entegrasyonları ile gelişmiş yönetim özelliklerine sahiptir.  

## 🚀 Özellikler  

### ✨ Kullanıcı Özellikleri  
- **Üye Olma & Giriş Yapma**: Kullanıcılar üye olup giriş yapabilir.  
- **Uçuş Arama & Listeleme**: **MySQL** veritabanındaki verilere göre uçuşlar aranabilir ve listelenebilir.  
- **Bilet Satın Alma**: Kullanıcılar giriş yaptıktan sonra yolcu bilgilerini girerek ve uçuş sınıfını seçerek uçak bileti satın alabilir.  
- **Harita Üzerinde Uçuş Rotası Görüntüleme**:  
  - **Mapbox API** kullanılarak iki lokasyon arasındaki uçuş rotası harita üzerinde gösterilir.  
- **Hesabım Sayfası**:  
  - Üyelik bilgilerini görüntüleyip düzenleme  
  - Yapılan rezervasyonları görüntüleme  

### 🛠️ Admin Özellikleri  
- **Admin Paneli**:  
  - Uçuşlar için **CRUD (Create, Read, Update, Delete) işlemleri**  
- **SOAP Entegrasyonu**:  
  - Üye olmuş kullanıcıların bilgilerini **maskelenmiş şekilde** görüntüleyebilme  
- **gRPC Entegrasyonu**:  
  - Yolcuların bazı bilgilerini **maskelenmiş şekilde** görüntüleyebilme  

## 🏗️ Teknolojiler & Araçlar  
| Teknoloji | Açıklama |  
|-----------|---------|  
| **ASP.NET Core MVC** | Web uygulaması geliştirme |  
| **MySQL** | Veritabanı yönetimi |  
| **Mapbox API** | Uçuş rotalarını harita üzerinde göstermek için |  
| **SOAP** | Kullanıcı verilerini güvenli bir şekilde almak için |  
| **gRPC** | Yolcu bilgilerini işlemek için |  
| **AJAX** | Dinamik içerik yükleme |  


