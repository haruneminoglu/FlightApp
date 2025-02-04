document.addEventListener("DOMContentLoaded", function () {
    const oneWay = document.getElementById("oneWay");
    const roundTrip = document.getElementById("roundTrip");
    const returnDate = document.getElementById("returnDate");

    oneWay.addEventListener("change", function () {
        if (oneWay.checked) {
            returnDate.disabled = true;
            returnDate.value = ""; // Dönüş tarihini temizle
        }
    });

    roundTrip.addEventListener("change", function () {
        if (roundTrip.checked) {
            returnDate.disabled = false;
            // AJAX çağrısı yaparak dönüş tarihi alanını aktif hale getirebiliriz.
            $.ajax({
                url: '/Home/EnableReturnDate',
                type: 'GET',
                success: function (data) {
                    // Dönüş tarihi alanını aktif yap
                    returnDate.disabled = false;
                }
            });
        }
    });
});
