<template>
    <div class="offcanvas offcanvas-end" tabindex="-1" id="appSidebar" aria-labelledby="sidebarLabel" data-bs-backdrop="false" data-bs-scroll="true">
        <!--Nombre de la empresa y X para cerrarlo-->
        <div class="offcanvas-header py-4 my-3">
                <h5 id="sidebarLabel">Empresa ABC</h5>
                <button type="button" class="btn-close shadow-none" data-bs-dismiss="offcanvas" aria-label="Cerrar"></button>
        </div>
        <div class="offcanvas-body">
            <!--Nombre e icono del usuario-->
            <div>
                <i class="bi bi-person-circle fs-2 me-3"></i>
                <span>Paula Lopez</span>
            </div>

            <ul class="list-unstyled ps-0" id="sidebar-accordion"> 
                <li class="border-top my-3"></li>
                <!--Para volver a la página principal-->
                <li class="my-3">
                    <RouterLink class="link-body-emphasis text-decoration-none" to="/Home">
                        <div class="gap-2 px-2 py-2 sidebar-item" data-bs-dismiss="offcanvas">
                            <i class="bi bi-house me-3"></i>
                            <span>Página principal</span>
                        </div>
                    </RouterLink>
                </li>
                <!--Opciones para empresa-->
                <li class="my-3">
                    <button
                        class="nav-link d-flex w-100 gap-2 px-2 py-2 sidebar-item collapsed"
                        data-bs-toggle="collapse" data-bs-target="#company-collapse" aria-expanded="false" aria-controls="company-collapse">
                        <i class="bi bi-buildings me-3"></i>
                        <span class="flex-grow-1 text-start">Empresas</span>
                        <i class="bi bi-chevron-right caret"></i>
                    </button>
                    <div class="collapse" id="company-collapse" data-bs-parent="#sidebar-accordion">
                        <ul class="list-unstyled ms-4 ps-2 small">
                            <li>
                                <RouterLink class="nav-link my-3" to="/Home">
                                    <span class="px-2 py-2 sidebar-item" data-bs-dismiss="offcanvas">Ver Empresas</span>
                                </RouterLink>
                            </li>
                            <li>
                                <RouterLink class="nav-link my-3" to="/CrearEmpresa">
                                    <span class="px-2 py-2 sidebar-item" data-bs-dismiss="offcanvas">Crear Empresa</span>
                                </RouterLink>
                            </li>
                        </ul>
                    </div>
                </li>
                <!--Opciones para empleados-->
                <li class="my-3">
                    <button
                        class="nav-link d-flex w-100 gap-2 px-2 py-2 sidebar-item collapsed"
                        data-bs-toggle="collapse" data-bs-target="#employee-collapse" aria-expanded="false" aria-controls="employee-collapse">
                        <i class="bi bi-people me-3"></i>
                        <span class="flex-grow-1 text-start">Empleados</span>
                        <i class="bi bi-chevron-right caret"></i>
                    </button>
                    <div class="collapse" id="employee-collapse" data-bs-parent="#sidebar-accordion">
                        <ul class="list-unstyled ms-4 ps-2 small">
                            <li>
                                <RouterLink class="nav-link my-3" to="/Home">
                                    <span class="px-2 py-2 sidebar-item" data-bs-dismiss="offcanvas">Ver Empleados</span>
                                </RouterLink>
                            </li>
                            <li>
                                <RouterLink class="nav-link my-3" to="/Home">
                                    <span class="px-2 py-2 sidebar-item" data-bs-dismiss="offcanvas">Crear Empleados</span>
                                </RouterLink>
                            </li>
                        </ul>
                    </div>
                </li>
                <!--Opciones para timesheets-->
                <li class="my-3">
                    <button
                        class="nav-link d-flex w-100 gap-2 px-2 py-2 sidebar-item collapsed"
                        data-bs-toggle="collapse" data-bs-target="#timesheet-collapse" aria-expanded="false" aria-controls="timesheet-collapse">
                        <i class="bi bi-calendar-week me-3"></i>
                        <span class="flex-grow-1 text-start">Timesheets</span>
                        <i class="bi bi-chevron-right caret"></i>
                    </button>
                    <div class="collapse" id="timesheet-collapse" data-bs-parent="#sidebar-accordion">
                        <ul class="list-unstyled ms-4 ps-2 small">
                            <li>
                                <RouterLink class="nav-link my-3" to="/Home">
                                    <span class="px-2 py-2 sidebar-item" data-bs-dismiss="offcanvas">Ver Timesheets</span>
                                </RouterLink>
                            </li>
                            <li>
                                <RouterLink class="nav-link my-3" to="/Home">
                                    <span class="px-2 py-2 sidebar-item" data-bs-dismiss="offcanvas">Aprobar Timesheets</span>
                                </RouterLink>
                            </li>
                            <li>
                                <RouterLink class="nav-link my-3" to="/Home">
                                    <span class="px-2 py-2 sidebar-item" data-bs-dismiss="offcanvas">Crear Timesheet</span>
                                </RouterLink>
                            </li>
                        </ul>
                    </div>
                </li>
                <!--Opciones para planilla-->
                <li class="my-3">
                    <button
                        class="nav-link d-flex w-100 gap-2 px-2 py-2 sidebar-item collapsed"
                        data-bs-toggle="collapse" data-bs-target="#payroll-collapse" aria-expanded="false" aria-controls="payroll-collapse">
                        <i class="bi bi-calculator me-3"></i>
                        <span class="flex-grow-1 text-start">Planilla</span>
                        <i class="bi bi-chevron-right caret"></i>
                    </button>
                    <div class="collapse" id="payroll-collapse" data-bs-parent="#sidebar-accordion">
                        <ul class="list-unstyled ms-4 ps-2 small">
                            <li>
                                <RouterLink class="nav-link my-3" to="/Home">
                                    <span class="px-2 py-2 sidebar-item" data-bs-dismiss="offcanvas">Historial de planillas</span>
                                </RouterLink>
                            </li>
                            <li>
                                <RouterLink class="nav-link my-3" to="/Home">
                                    <span class="px-2 py-2 sidebar-item" data-bs-dismiss="offcanvas">Crear planilla</span>
                                </RouterLink>
                            </li>
                            <li>
                                <RouterLink class="nav-link my-3" to="/Home">
                                    <span class="px-2 py-2 sidebar-item" data-bs-dismiss="offcanvas">Ver beneficios y deducciones</span>
                                </RouterLink>
                            </li>
                            <li>
                                <RouterLink class="nav-link my-3" to="/Home">
                                    <span class="px-2 py-2 sidebar-item" data-bs-dismiss="offcanvas">Crear beneficios y deducciones</span>
                                </RouterLink>
                            </li>
                            <li>
                                <RouterLink class="nav-link my-3" to="/Home">
                                    <span class="px-2 py-2 sidebar-item" data-bs-dismiss="offcanvas">Asignar deducciones</span>
                                </RouterLink>
                            </li>
                            <li>
                                <RouterLink class="nav-link my-3" to="/Home">
                                    <span class="px-2 py-2 sidebar-item" data-bs-dismiss="offcanvas">Seleccionar beneficios</span>
                                </RouterLink>
                            </li>
                        </ul>
                    </div>
                </li>
                <!--Opciones para pagos-->
                <li class="my-3">
                    <button
                        class="nav-link d-flex w-100 gap-2 px-2 py-2 sidebar-item collapsed"
                        data-bs-toggle="collapse" data-bs-target="#payment-collapse" aria-expanded="false" aria-controls="payment-collapse">
                        <i class="bi bi-credit-card me-3"></i>
                        <span class="flex-grow-1 text-start">Pagos</span>
                        <i class="bi bi-chevron-right caret"></i>
                    </button>
                    <div class="collapse" id="payment-collapse" data-bs-parent="#sidebar-accordion">
                        <ul class="list-unstyled ms-4 ps-2 small">
                            <li>
                                <RouterLink class="nav-link my-3" to="/Home">
                                    <span class="px-2 py-2 sidebar-item" data-bs-dismiss="offcanvas">Historial de pagos</span>
                                </RouterLink>
                            </li>
                            <li>
                                <RouterLink class="nav-link my-3" to="/Home">
                                    <span class="px-2 py-2 sidebar-item" data-bs-dismiss="offcanvas">Crear pago</span>
                                </RouterLink>
                            </li>
                        </ul>
                    </div>
                </li>
                <!--Opciones para reportes-->
                <li class="my-3">
                    <button
                        class="nav-link d-flex w-100 gap-2 px-2 py-2 sidebar-item collapsed"
                        data-bs-toggle="collapse" data-bs-target="#report-collapse" aria-expanded="false" aria-controls="report-collapse">
                        <i class="bi bi-bar-chart me-3"></i>
                        <span class="flex-grow-1 text-start">Reportes</span>
                        <i class="bi bi-chevron-right caret"></i>
                    </button>
                    <div class="collapse" id="report-collapse" data-bs-parent="#sidebar-accordion">
                        <ul class="list-unstyled ms-4 ps-2 small">
                            <li>
                                <RouterLink class="nav-link my-3" to="/Home">
                                    <span class="px-2 py-2 sidebar-item" data-bs-dismiss="offcanvas">Ver Reportes</span>
                                </RouterLink>
                            </li>
                            <li>
                                <RouterLink class="nav-link my-3" to="/Home">
                                    <span class="px-2 py-2 sidebar-item" data-bs-dismiss="offcanvas">Crear Reporte</span>
                                </RouterLink>
                            </li>
                        </ul>
                    </div>
                </li>
                <li class="border-top my-3"></li> 
                <!--Botón para cerrar sesión en rojo-->    
                <li class="my-3">
                    <RouterLink class="nav-link link-danger" to="/">
                        <div class="gap-2 px-2 py-2" data-bs-dismiss="offcanvas">
                            <i class="bi bi-box-arrow-right me-3"></i>
                            <span>Cerrar sesión</span>
                        </div>
                    </RouterLink>
                </li>
            </ul>
        </div>
    </div>
</template>

<script>
</script>

<style lang="scss" scoped>

    .sidebar-item:hover{
        border-radius: .5rem;
        background-color: rgba(35,77,52,.12);
        color: #1b3d2a;
        cursor: pointer;
    }

</style>