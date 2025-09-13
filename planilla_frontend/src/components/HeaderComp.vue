<template>
    <header class="d-flex justify-content-between align-items-center py-3 mb-4 px-2">
        <a href="/" class="d-flex align-items-center mb-3 mb-md-0 me-md-auto link-body-emphasis text-decoration-none">
            <img src="@/assets/logo.png" alt="Logo" width="60" height="60" class="me-2">
            <span class="fs-4">Onigiri Intelligent Solutions</span>
        </a>
        <ul class="nav nav-underline d-flex align-items-center gap-3">
            <li class="nav-item">
                <div class="dropdown text-end">
                    <a href="#" class="d-block link-body-emphasis text-decoration-none dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
                        <span class="fs-5">Empresa ABC</span>
                    </a>
                    <ul class="dropdown-menu text-small" style="">
                        <li><a class="dropdown-item" href="#">Empresa DEF</a></li>
                        <li><a class="dropdown-item" href="#">Empresa GHI</a></li>
                    </ul>
                </div>
            </li> 
            <li class="nav-item">
                <router-link to="/" class="text-dark text-decoration-none p-0">
                    <i class="bi bi-house-fill fs-4"></i>
                </router-link>
            </li>
            <li class="nav-item">
                <i class="bi bi-bell-fill fs-4"></i>
            </li>
            <li class="nav-item">
                <div class="dropdown text-end">
                    <a href="#" class="d-block link-body-emphasis text-decoration-none" data-bs-toggle="dropdown" aria-expanded="false">
                        <i class="bi bi-person-circle fs-4"></i>
                    </a>
                    <ul class="dropdown-menu text-small" style="">
                        <li><a class="dropdown-item" href="#">Configuración</a></li>
                        <li><hr class="dropdown-divider"></li>
                        <li><a class="dropdown-item" href="#">Cerrar sesión</a></li>
                    </ul>
                </div>
            </li> 
        </ul> 
    </header>
</template>

<script>
    export default {
        setup() {
            return {}
        },
    }
</script>

<style lang="scss" scoped>

</style>