// Script para notificações e alertas usando SweetAlert2

// Exibe uma notificação de sucesso
function showSuccessNotification(message) {
    Swal.fire({
        title: 'Sucesso!',
        text: message,
        icon: 'success',
        confirmButtonText: 'OK',
        confirmButtonColor: '#3085d6'
    });
}

// Exibe uma notificação de erro
function showErrorNotification(message) {
    Swal.fire({
        title: 'Erro!',
        text: message,
        icon: 'error',
        confirmButtonText: 'OK',
        confirmButtonColor: '#d33'
    });
}

// Exibe uma notificação de alerta/informação
function showInfoNotification(message) {
    Swal.fire({
        title: 'Atenção!',
        text: message,
        icon: 'info',
        confirmButtonText: 'OK',
        confirmButtonColor: '#3085d6'
    });
}

// Exibe uma caixa de diálogo de confirmação
function showConfirmationDialog(title, text, confirmText, cancelText, confirmCallback) {
    Swal.fire({
        title: title,
        text: text,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: confirmText || 'Sim',
        cancelButtonText: cancelText || 'Cancelar',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            if (typeof confirmCallback === 'function') {
                confirmCallback();
            }
        }
    });
}

// Exibe uma notificação de sucesso na parte superior direita (toast)
function showSuccessToast(message) {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer)
            toast.addEventListener('mouseleave', Swal.resumeTimer)
        }
    });
    
    Toast.fire({
        icon: 'success',
        title: message
    });
}

// Exibe uma notificação de erro na parte superior direita (toast)
function showErrorToast(message) {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer)
            toast.addEventListener('mouseleave', Swal.resumeTimer)
        }
    });
    
    Toast.fire({
        icon: 'error',
        title: message
    });
}
