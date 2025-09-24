<template>
    <div class="offcanvas offcanvas-end" tabindex="-1" id="appSidebar" aria-labelledby="sidebarLabel" data-bs-backdrop="false" data-bs-scroll="true">
        <div class="offcanvas-header py-4 my-3">
                <h5 id="sidebarLabel">Empresa ABC</h5>
                <button type="button" class="btn-close shadow-none" data-bs-dismiss="offcanvas" aria-label="Cerrar"></button>
        </div>
        <div class="offcanvas-body">
            <div>
                <i class="bi bi-person-circle fs-2"></i>
                <span>Paula Lopez</span>
            </div>

            <ul class="list-unstyled ps-0"> 
                <li class="border-top my-3"></li>
                <li class="mb-1">
                    <i class="bi bi-house"></i>
                    <RouterLink
                        class="link-body-emphasis d-inline-flex text-decoration-none rounded" 
                        to="/" @click="closeSidebar">Página principal
                    </RouterLink>
                </li>
                <li class="mb-1">
                    <i class="bi bi-house"></i>
                    <button
                        class="btn btn-toggle d-inline-flex align-items-center rounded border-0 collapsed"
                        data-bs-toggle="collapse" data-bs-target="#company-collapse" aria-expanded="true">
                        Empresas
                    </button>
                    <div class="collapse show" id="company-collapse">
                        <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
                            <li>
                                <RouterLink
                                    class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                    to="/" @click="closeSidebar">Ver Empresas
                                </RouterLink>
                            </li>
                            <li>
                                <RouterLink
                                    class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                    to="/" @click="closeSidebar">Crear Empresa
                                </RouterLink>
                            </li>
                        </ul>
                    </div>
                </li>
                <li class="mb-1">
                    <i class="bi bi-house"></i>
                    <button
                        class="btn btn-toggle d-inline-flex align-items-center rounded border-0 collapsed"
                        data-bs-toggle="collapse" data-bs-target="#employee-collapse" aria-expanded="true">
                        Empleados
                    </button>
                    <div class="collapse show" id="employee-collapse">
                        <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
                            <li>
                                <RouterLink
                                    class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                    to="/" @click="closeSidebar">Ver Empleados
                                </RouterLink>
                            </li>
                            <li>
                                <RouterLink
                                    class="link-body-emphasis d-inline-flex text-decoration-none rounded"
                                    to="/" @click="closeSidebar">Crear Empleados
                                </RouterLink>
                            </li>
                        </ul>
                    </div>
                </li>
                <li class="border-top my-3"></li> 
                
                <li class="mb-1">
                    <i class="bi bi-house"></i>
                    <RouterLink
                        class="link-body-emphasis d-inline-flex text-decoration-none rounded" 
                        to="/" @click="closeSidebar">Cerrar sesión
                    </RouterLink>
                </li>
            </ul>
        </div>
    </div>
</template>

<script>
</script>

<style lang="scss" scoped>
</style>