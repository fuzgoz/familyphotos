const dropzone = document.querySelector('body');

dropzone.addEventListener('dragover', event => {
    event.preventDefault();
});

dropzone.addEventListener('drop', event => {
    event.preventDefault();
    dropzone.classList.remove('dropactive');

    const file = event.dataTransfer.files[0]; // Csak az első fájl
    if (!file) {
        alert("Nincs fájl.");
        return;
    }

    const formData = new FormData();
    formData.append('image', file);

    const folderId = window.location.pathname.split('/').pop();

    fetch(`/Image/Upload/${folderId}`, {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Sikertelen fájlfeltöltés.');
            }
            return response.json();
        })
        .then(data => {
            alert('Fénykép sikeresen feltöltve!');
            // Újratölti az oldalt, hogy frissítsd a képeket az adatbázisból
            window.location.reload();
        })
        .catch(error => {
            console.error('Hiba:', error);
            alert('Hiba a fényképfeltöltés közben. Kérlek próbáld újra később.');
        });
});