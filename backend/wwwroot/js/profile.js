$(document).ready(function () {
    // Düzenle butonuna tıklama olayını dinliyoruz
    $(".edit-button").click(function () {
        const field = $(this).data("field"); // Düzenlenecek alan
        const value = $("#" + field.toLowerCase()).text().trim(); // Mevcut değeri al (trim ile boşlukları temizle)
        $("#editField").val(value); // Modal giriş alanına mevcut değeri yaz
        $("#editModal").data("field", field); // Modal'a hangi alanın düzenlendiğini ekle
        $("#editModal").modal('show'); // Modal'ı aç
    });

    // Kaydet butonuna tıklama olayını dinliyoruz
    $("#saveEdit").click(function () {
        const field = $("#editModal").data("field"); // Düzenlenen alan
        const newValue = $("#editField").val().trim(); // Yeni değer

        if (newValue === "") {
            alert("Alan boş bırakılamaz!"); // Değer boşsa uyarı göster
            return;
        }

        // AJAX isteği gönder
        $.ajax({
            url: '/Account/UpdateProfile',
            type: 'POST',
            data: { userId: userId, fieldName: field, newValue: newValue },
            success: function (response) {
                if (response.success) {
                    $("#" + field.toLowerCase()).text(newValue); // Başarılıysa alanı güncelle
                    $("#editModal").modal('hide'); // Modal'ı kapat
                } else {
                    alert("Değişiklik kaydedilemedi."); // Başarısızlık mesajı
                }
            },
            error: function () {
                alert("Bir hata oluştu. Lütfen tekrar deneyin."); // Hata mesajı
            }
        });
    });
});
