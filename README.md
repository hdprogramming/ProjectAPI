Markdown

# ProjectCLVue 🚀

Bu proje, **Vue 3** ve **Vite** kullanılarak geliştirilmiş modern bir frontend uygulamasıdır. İçerisinde dosya yönetimi, metin editörü (Quill) ve güvenli veri işleme yapıları barındırır.

> ⚠️ **DİKKAT: Geliştirme Süreci Devam Ediyor!** > Bu proje şu anda aktif geliştirme aşamasındadır (WIP). Bazı özellikler tamamlanmamış olabilir veya ciddi hatalar (bug) içerebilir. Kullanırken veya incelerken bunu göz önünde bulundurunuz.

## 🛠️ Önerilen IDE Kurulumu

Bu proje için en verimli geliştirme ortamı:

- [VS Code](https://code.visualstudio.com/) 
- [Vue - Official](https://marketplace.visualstudio.com/items?itemName=Vue.volar) eklentisi.
  - *Not: Eğer yüklüyse `Vetur` eklentisini devre dışı bırakın.*

## 🌐 Önerilen Tarayıcı Kurulumu

Vue 3 reaktivite sistemini ve değişkenleri rahatça inceleyebilmek için:

### Chromium Tabanlı Tarayıcılar (Chrome, Edge, Brave vb.)
1. [Vue.js devtools](https://chrome.google.com/webstore/detail/vuejs-devtools/nhdogjmejiglipccpnnnanhbledajbpd) eklentisini yükleyin.
2. **Chrome DevTools** Ayarlarını açın (F12 -> Ayarlar).
3. "Console" bölümü altındaki **"Enable custom formatters"** seçeneğini işaretleyin.

### Firefox
1. [Vue.js devtools](https://addons.mozilla.org/en-US/firefox/addon/vue-js-devtools/) eklentisini yükleyin.
2. **Firefox DevTools** Ayarlarını açın.
3. **"Enable custom formatters"** seçeneğini aktif hale getirin.

## ⚙️ Proje Kurulumu

Projeyi klonladıktan sonra gerekli bağımlılıkları yüklemek için terminalde şu komutu çalıştırın:

```sh
npm install
Geliştirme Modunda Çalıştırma (Hot-Reload)
Projeyi yerel sunucuda başlatmak ve anlık değişiklikleri görmek için:

Bash

npm run dev
Canlı Ortam İçin Derleme (Production Build)
Projeyi yayına hazırlamak, sıkıştırmak ve optimize etmek için:

Bash

npm run build
📂 Özellikler (Özet)
Vue 3 Composition API: <script setup> yapısı ile temiz kod.

XSS Koruması: DOMPurify ile güvenli HTML render işlemleri.

Dosya Yöneticisi: Resim yükleme, silme ve düzenleme modülü.

Zengin Metin Editörü: Quill Editor entegrasyonu.

Router Yapısı: ID tabanlı dinamik sayfa yönlendirmeleri.

🔧 Konfigürasyon
Vite ayarlarını özelleştirmek için Vite Configuration Reference sayfasına bakabilirsiniz.


### Nasıl Kullanırsın?
1.  Proje klasöründe `README.md` adında bir dosya oluştur.
2.  Yukarıdaki kodu kopyalayıp içine yapıştır.
3.  Kaydet.