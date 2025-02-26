const themeToggleButton = document.getElementById('theme-toggle');

function toggleTheme() {
    document.body.classList.toggle('dark-theme');
    if (document.body.classList.contains('dark-theme')) {
        localStorage.setItem('theme', 'dark');
    } else {
        localStorage.setItem('theme', 'light');
    }
}

window.addEventListener('load', () => {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark') {
        document.body.classList.add('dark-theme');
    }
});

themeToggleButton.addEventListener('click', toggleTheme);

$(document).ready(function () {
    $('#modalAuthorization').on('show.bs.modal', function () {
        $('#modalRegister').modal('hide');
        $('#modalForgotPassword').modal('hide');
    });

    $('#modalRegister').on('show.bs.modal', function () {
        $('#modalAuthorization').modal('hide');
        $('#modalForgotPassword').modal('hide');
    });

    $('#modalForgotPassword').on('show.bs.modal', function () {
        $('#modalAuthorization').modal('hide');
        $('#modalRegister').modal('hide');
    });
});

function openAuthModal() {
    $.ajax({
        url: '/Auth/ShowAuthorizationModal',
        type: 'GET',
        success: function (data) {
            $('#modalContainer').html(data);

            var modal = new bootstrap.Modal(document.getElementById('modalAuthorization'));
            modal.show();
        }
    });
}

function openRegisterModal() {
    $.ajax({
        url: '/Auth/ShowRegistrationModal',
        type: 'GET',
        success: function (data) {
            $('#modalContainer').html(data);

            var modal = new bootstrap.Modal(document.getElementById('modalRegistration'));
            modal.show();
        }
    });
}

function openForgotPasswordModal() {
    $.ajax({
        url: '/Auth/ShowForgotPasswordModal',
        type: 'GET',
        success: function (data) {
            $('#modalContainer').html(data);

            var modal = new bootstrap.Modal(document.getElementById('modalForgotPassword'));
            modal.show();
        }
    });
}

function openEditProfileDataModal() {
    $.ajax({
        url: '/Profile/ShowEditProfileDataModal',
        type: 'GET',
        success: function (data) {
            $('#modalContainer').html(data);

            var modal = new bootstrap.Modal(document.getElementById('modalEditProfile'));
            modal.show();
        }
    });
}

function logout() {
    $.ajax({
        url: '/Auth/Logout',
        type: 'POST',
        success: function (response) {
            alert('Вы вышли из аккаунта');
            window.location.href = "/";
        }
    });
}

function changeProfilePicture() {
    var url = '/Profile/ChangeProfilePictureGet';
    $.ajax({
        method: "GET",
        url: url,
        success: function (response) {
            $('#modalContainer').html(response);

            var modal = new bootstrap.Modal(document.getElementById('profilePictureModal'));
            modal.show();

            fileName();

            $('#buttonSubmitChanging').on('click', function () {
                var fileUpload = $("#File").get(0);
                var files = fileUpload.files;

                if (files.length === 0) {
                    alert("Выберите файл");
                    return;
                }

                var fileData = new FormData();
                for (var i = 0; i < files.length; i++) {
                    fileData.append(files[i].name, files[i]);
                }

                var urlPost = '/Profile/ChangeProfilePicturePost';
                $.ajax({
                    url: urlPost,
                    type: "POST",
                    contentType: false,
                    processData: false,
                    data: fileData,
                    success: function (response) {
                        if (response.success) {
                            $('#profilePictureModal').modal('hide');
                            alert("Аватар профиля добавлен.");
                            window.location.reload();
                        }
                        else {
                            alert("Произошла ошибка.");
                        }
                    },
                    error: function (jqXHR) {
                        if (jqXHR.status == 403) {
                            alert('Вы не авторизованы');
                        }
                    },
                }).fail(function (xhr, status, p3) { alert(xhr.responseText); });
            });
        },
        error: function (jqXHR, textStatus) {
            if (jqXHR.status == 403) {
                alert('Вы не авторизованы');
            }
        },
    });
}

function fileName() {
    $('.input-file').each(function () {
        var $input = $(this),
            $label = $input.next('label'),
            labelVal = $label.html();
        $input.on('change', function (e) {
            var fileName = e.target.value.split('\\').pop();

            if (fileName) {
                $label.find('span').html(fileName);
            }
            else {
                $label.html(labelVal);
            }
        });
    });
};