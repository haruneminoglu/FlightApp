function loadUsers() {
    console.log('Kullanıcılar yükleniyor...');
    
    fetch('/AdminUsers/GetUsers')
        .then(response => response.json())
        .then(data => {
            console.log('Gelen ham veri:', data);
            
            const tbody = document.getElementById('userTableBody');
            tbody.innerHTML = '';
            
            if (!data || data.length === 0) {
                tbody.innerHTML = '<tr><td colspan="4" class="text-center">Hiç kullanıcı bulunamadı.</td></tr>';
                return;
            }
            
            data.forEach(user => {
                console.log('Tek kullanıcı verisi:', user);
                tbody.innerHTML += `
                    <tr>
                        <td>${user.userId}</td>
                        <td>${user.firstname}</td>
                        <td>${user.lastname}</td>
                        <td>${user.maskedEmail}</td>
                    </tr>
                `;
            });
        })
        .catch(error => {
            console.error('Veri yükleme hatası:', error);
            document.getElementById('userTableBody').innerHTML = `
                <tr>
                    <td colspan="4" class="text-center text-danger">
                        Veriler yüklenirken bir hata oluştu: ${error.message}
                    </td>
                </tr>
            `;
        });
}

document.addEventListener('DOMContentLoaded', loadUsers);