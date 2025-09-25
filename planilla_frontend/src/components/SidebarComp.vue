<template>
  <div
    class="offcanvas offcanvas-end"
    tabindex="-1"
    id="appSidebar"
    aria-labelledby="sidebarLabel"
    data-bs-backdrop="false"
    data-bs-scroll="true"
  >
    <!--Nombre de la empresa y X para cerrarlo-->
    <div class="offcanvas-header py-4 my-3">
      <h5 id="sidebarLabel">Empresa ABC</h5>
      <button
        type="button"
        class="btn-close shadow-none"
        data-bs-dismiss="offcanvas"
        aria-label="Cerrar"
      ></button>
    </div>

    <div class="offcanvas-body">
      <!--Nombre e icono del usuario-->
      <div>
        <i class="bi bi-person-circle fs-2 me-3"></i>
        <span>Paula Lopez</span>
      </div>

      <ul class="list-unstyled ps-0">
        <li class="border-top my-3"></li>

        <!--Para volver a la página principal-->
        <li class="my-3">
          <RouterLink
            class="link-body-emphasis text-decoration-none"
            to="/Home"
            @click="closeSidebar"
          >
            <i class="bi bi-house me-3"></i>
            <span>Página principal</span>
          </RouterLink>
        </li>

        <!--Opciones para empresa-->
        <li class="my-3">
          <button
            class="nav-link d-flex w-100"
            data-bs-toggle="collapse"
            data-bs-target="#company-collapse"
            aria-expanded="false"
          >
            <i class="bi bi-buildings me-3"></i>
            <span class="flex-grow-1 text-start">Empresas</span>
            <i class="bi bi-chevron-right caret"></i>
          </button>
          <div class="collapse" id="company-collapse">
            <ul class="list-unstyled ms-4 ps-2 small">
              <li>
                <RouterLink class="nav-link my-2" to="/Home" @click="closeSidebar"
                  >Ver Empresas
                </RouterLink>
              </li>
              <li>
                <RouterLink class="nav-link my-2" to="/CrearEmpresa" @click="closeSidebar"
                  >Crear Empresa
                </RouterLink>
              </li>
            </ul>
          </div>
        </li>

        <!--Opciones para empleados-->
        <li class="my-3">
          <button
            class="nav-link d-flex w-100"
            data-bs-toggle="collapse"
            data-bs-target="#employee-collapse"
            aria-expanded="false"
          >
            <i class="bi bi-people me-3"></i>
            <span class="flex-grow-1 text-start">Empleados</span>
            <i class="bi bi-chevron-right caret"></i>
          </button>
          <div class="collapse" id="employee-collapse">
            <ul class="list-unstyled ms-4 ps-2 small">
              <li>
                <RouterLink class="nav-link my-2" to="/Home" @click="closeSidebar"
                  >Ver Empleados
                </RouterLink>
              </li>
              <li>
                <RouterLink class="nav-link my-2" to="/Home" @click="closeSidebar"
                  >Crear Empleados
                </RouterLink>
              </li>
            </ul>
          </div>
        </li>

        <!--Opciones para timesheets-->
        <li class="my-3">
          <button
            class="nav-link d-flex w-100"
            data-bs-toggle="collapse"
            data-bs-target="#timesheet-collapse"
            aria-expanded="false"
          >
            <i class="bi bi-calendar-week me-3"></i>
            <span class="flex-grow-1 text-start">Timesheets</span>
            <i class="bi bi-chevron-right caret"></i>
          </button>
          <div class="collapse" id="timesheet-collapse">
            <ul class="list-unstyled ms-4 ps-2 small">
              <li>
                <RouterLink class="nav-link my-2" to="/Home" @click="closeSidebar"
                  >Ver Timesheets
                </RouterLink>
              </li>
              <li>
                <RouterLink class="nav-link my-2" to="/Home" @click="closeSidebar"
                  >Aprobar Timesheets
                </RouterLink>
              </li>
              <li>
                <RouterLink class="nav-link my-2" to="/Home" @click="closeSidebar"
                  >Crear Timesheet
                </RouterLink>
              </li>
            </ul>
          </div>
        </li>

        <!--Opciones para planilla-->
        <li class="my-3">
          <button
            class="nav-link d-flex w-100"
            data-bs-toggle="collapse"
            data-bs-target="#payroll-collapse"
            aria-expanded="false"
          >
            <i class="bi bi-calculator me-3"></i>
            <span class="flex-grow-1 text-start">Planilla</span>
            <i class="bi bi-chevron-right caret"></i>
          </button>
          <div class="collapse" id="payroll-collapse">
            <ul class="list-unstyled ms-4 ps-2 small">
              <li>
                <RouterLink class="nav-link my-2" to="/Home" @click="closeSidebar"
                  >Historial de planillas
                </RouterLink>
              </li>
              <li>
                <RouterLink class="nav-link my-2" to="/Home" @click="closeSidebar"
                  >Crear planilla
                </RouterLink>
              </li>
              <li>
                <RouterLink class="nav-link my-2" to="/Home" @click="closeSidebar"
                  >Ver beneficios y deducciones
                </RouterLink>
              </li>
              <li>
                <RouterLink
                  class="nav-link my-2"
                  to="/CrearBeneficiosDeducciones"
                  @click="closeSidebar"
                  >Crear beneficios y deducciones
                </RouterLink>
              </li>
              <li>
                <RouterLink class="nav-link my-2" to="/Home" @click="closeSidebar"
                  >Asignar deducciones
                </RouterLink>
              </li>
              <li>
                <RouterLink class="nav-link my-2" to="/Home" @click="closeSidebar"
                  >Seleccionar beneficios
                </RouterLink>
              </li>
            </ul>
          </div>
        </li>

        <!--Opciones para pagos-->
        <li class="my-3">
          <button
            class="nav-link d-flex w-100"
            data-bs-toggle="collapse"
            data-bs-target="#payment-collapse"
            aria-expanded="false"
          >
            <i class="bi bi-credit-card me-3"></i>
            <span class="flex-grow-1 text-start">Pagos</span>
            <i class="bi bi-chevron-right caret"></i>
          </button>
          <div class="collapse" id="payment-collapse">
            <ul class="list-unstyled ms-4 ps-2 small">
              <li>
                <RouterLink class="nav-link my-2" to="/Home" @click="closeSidebar"
                  >Historial de pagos
                </RouterLink>
              </li>
              <li>
                <RouterLink class="nav-link my-2" to="/Home" @click="closeSidebar"
                  >Crear pago
                </RouterLink>
              </li>
            </ul>
          </div>
        </li>

        <!--Opciones para reportes-->
        <li class="my-3">
          <button
            class="nav-link d-flex w-100"
            data-bs-toggle="collapse"
            data-bs-target="#report-collapse"
            aria-expanded="false"
          >
            <i class="bi bi-bar-chart me-3"></i>
            <span class="flex-grow-1 text-start">Reportes</span>
            <i class="bi bi-chevron-right caret"></i>
          </button>
          <div class="collapse" id="report-collapse">
            <ul class="list-unstyled ms-4 ps-2 small">
              <li>
                <RouterLink class="nav-link my-2" to="/Home" @click="closeSidebar"
                  >Ver Reportes
                </RouterLink>
              </li>
              <li>
                <RouterLink class="nav-link my-2" to="/Home" @click="closeSidebar"
                  >Crear Reporte
                </RouterLink>
              </li>
            </ul>
          </div>
        </li>

        <li class="border-top my-3"></li>

        <!--Botón para cerrar sesión en rojo-->
        <li class="my-3">
          <RouterLink
            class="nav-link link-danger text-decoration-none"
            to="/"
            @click="closeSidebar"
          >
            <i class="bi bi-box-arrow-right me-3"></i>
            <span>Cerrar sesión</span>
          </RouterLink>
        </li>
      </ul>
    </div>
  </div>
</template>

<script></script>

<style lang="scss" scoped></style>