// Archivo: wwwroot/js/ticket.js
window.imprimirTicketQR = (imagenBase64, total) => {
    console.log("Intentando imprimir ticket...");

    // Crear ventana popup
    var ventana = window.open('', 'PRINT', 'height=600,width=400');

    if (ventana) {
        ventana.document.write('<html><head><title>Ticket Preventa</title>');
        ventana.document.write('<style>');
        ventana.document.write('body { font-family: monospace; text-align: center; padding: 20px; }');
        ventana.document.write('.titulo { font-size: 1.5em; font-weight: bold; margin-bottom: 10px; }');
        ventana.document.write('img { max-width: 100%; height: auto; margin: 10px 0; }');
        ventana.document.write('.total { font-size: 2em; font-weight: bold; margin: 15px 0; border-top: 1px dashed #000; border-bottom: 1px dashed #000; padding: 10px 0; }');
        ventana.document.write('</style>');
        ventana.document.write('</head><body>');

        ventana.document.write('<div class="titulo">PRE-VENTA</div>');
        ventana.document.write('<img src="' + imagenBase64 + '" />');
        ventana.document.write('<div class="total">' + total + '</div>');
        ventana.document.write('<p>Presente este código en caja</p>');
        ventana.document.write('<small>' + new Date().toLocaleString() + '</small>');

        ventana.document.write('</body></html>');

        ventana.document.close();
        ventana.focus();

        // Esperar a que la imagen cargue antes de imprimir
        setTimeout(function () {
            ventana.print();
            ventana.close();
        }, 500);
    } else {
        alert("El navegador bloqueó la ventana. Por favor permite los pop-ups para este sitio.");
    }
};

window.vibrar = (milisegundos) => {
    if (navigator.vibrate) {
        navigator.vibrate(milisegundos); // Ej: 200ms
    }
};