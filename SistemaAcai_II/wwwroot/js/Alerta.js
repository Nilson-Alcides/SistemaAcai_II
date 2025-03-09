function abrirAlerta() {
    Swal.fire({
        icon: 'warning',
        title: 'Você precisa estar logado para favoritar. Deseja fazer login?',
        showDenyButton: true,
        showCancelButton: true,
        confirmButtonText: 'Fazer Login',
        denyButtonText: `Cadastrar-se`,
        cancelButtonText: 'Cancelar',
        confirmButtonColor: 'teal',
        denyButtonColor: '#cd853f',

    }).then((result) => {

        if (result.isConfirmed) {
            window.location.href = "view/login.html"
        } else if (result.isDenied) {
            window.location.href = "view/cad_usuario.html"
        }
    })
}

function abrirAlerta2() {
    Swal.fire({
        icon: 'warning',
        title: 'Você precisa estar logado para visitar o perfil. Deseja fazer login?',
        showDenyButton: true,
        showCancelButton: true,
        confirmButtonText: 'Fazer Login',
        denyButtonText: `Cadastrar-se`,
        cancelButtonText: 'Cancelar',
        confirmButtonColor: 'teal',
        denyButtonColor: '#cd853f',

    }).then((result) => {

        if (result.isConfirmed) {
            window.location.href = "view/login.html"
        } else if (result.isDenied) {
            window.location.href = "view/cad_usuario.html"
        }
    })
}

function abrirAlerta3() {
    Swal.fire({
        icon: 'warning',
        title: 'Você precisa estar logado para visualizar mais livros. Deseja fazer login?',
        showDenyButton: true,
        showCancelButton: true,
        confirmButtonText: 'Fazer Login',
        denyButtonText: `Cadastrar-se`,
        cancelButtonText: 'Cancelar',
        confirmButtonColor: 'teal',
        denyButtonColor: '#cd853f',

    }).then((result) => {

        if (result.isConfirmed) {
            window.location.href = "view/login.html"
        } else if (result.isDenied) {
            window.location.href = "view/cad_usuario.html"
        }
    })
}

function abrirAlerta4() {
    Swal.fire({
        icon: 'warning',
        title: 'Você precisa estar logado para ler mais. Deseja fazer login?',
        showDenyButton: true,
        showCancelButton: true,
        confirmButtonText: 'Fazer Login',
        denyButtonText: `Cadastrar-se`,
        cancelButtonText: 'Cancelar',
        confirmButtonColor: 'teal',
        denyButtonColor: '#cd853f',

    }).then((result) => {

        if (result.isConfirmed) {
            window.location.href = "view/login.html"
        } else if (result.isDenied) {
            window.location.href = "view/cad_usuario.html"
        }
    })
}

function abrirAlerta5() {
    Swal.fire({
        icon: 'warning',
        title: 'Você precisa estar logado para pesquisar. Deseja fazer login?',
        showDenyButton: true,
        showCancelButton: true,
        confirmButtonText: 'Fazer Login',
        denyButtonText: `Cadastrar-se`,
        cancelButtonText: 'Cancelar',
        confirmButtonColor: 'teal',
        denyButtonColor: '#cd853f',

    }).then((result) => {

        if (result.isConfirmed) {
            window.location.href = "view/login.html"
        } else if (result.isDenied) {
            window.location.href = "view/cad_usuario.html"
        }
    })
}

function abrirAlerta6() {
    Swal.fire({
        icon: 'warning',
        title: 'Você precisa estar logado para pesquisar por categoria. Deseja fazer login?',
        showDenyButton: true,
        showCancelButton: true,
        confirmButtonText: 'Fazer Login',
        denyButtonText: `Cadastrar-se`,
        cancelButtonText: 'Cancelar',
        confirmButtonColor: 'teal',
        denyButtonColor: '#cd853f',

    }).then((result) => {

        if (result.isConfirmed) {
            window.location.href = "view/login.html"
        } else if (result.isDenied) {
            window.location.href = "view/cad_usuario.html"
        }
    })
}

function AbrirFav() {

    const section = document.getElementById("fav_janela");

    section.classList.add('abrir');

    section.addEventListener('click', (e) => {
        if (e.target.id == 'fav_janela' || e.target.id == 'fechar') {
            section.classList.remove('abrir');
        }
    })

}

function AbrirDesFav() {
    const section = document.getElementById("desfav_janela");

    section.classList.add('abrir');

    section.addEventListener('click', (e) => {
        if (e.target.id == 'fechar_form' || e.target.id == 'fechar') {
            section.classList.remove('abrir');
        }
    })

}

function abrirRequi() {
    Swal.fire({
        icon: 'warning',
        title: 'Não é possível requisitar um livro que pertença a você.',
        showDenyButton: false,
        showCancelButton: false,
        confirmButtonText: 'OK',
        denyButtonText: `Cadastrar-se`,
        cancelButtonText: 'Cancelar',
        confirmButtonColor: 'teal',
        denyButtonColor: '#cd853f',

    })
}