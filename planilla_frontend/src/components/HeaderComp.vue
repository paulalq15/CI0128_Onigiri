<template>
  <header class="d-flex justify-content-between align-items-center py-3 mb-4 px-4" id="headerComp">
    <a
      class="d-flex align-items-center mb-3 mb-md-0 me-md-auto link-body-emphasis text-decoration-none"
    >
      <img src="@/assets/logo.png" alt="Logo" width="60" height="60" class="me-2" />
      <span class="fs-5 fw-medium">Onigiri Intelligent Solutions</span>
    </a>
    <ul class="nav nav-underline d-flex align-items-center gap-3">
      <li class="nav-item">
        <div class="dropdown text-end">
          <button
            class="btn btn-link dropdown-toggle text-dark text-decoration-none shadow-none"
            type="button"
            data-bs-toggle="dropdown"
            aria-expanded="false"
          >
            <span class="fs-5 fw-medium">Empresa ABC</span>
          </button>
          <ul class="dropdown-menu dropdown-menu-end" style="">
            <li><a class="dropdown-item" href="#">Empresa DEF</a></li>
            <li><a class="dropdown-item" href="#">Empresa GHI</a></li>
          </ul>
        </div>
      </li>
      <li class="nav-item">
        <button
          class="btn p-0 border-0 bg-transparent d-flex align-items-center shadow-none"
          type="button"
          data-bs-toggle="offcanvas"
          data-bs-target="#appSidebar"
          aria-controls="appSidebar"
        >
          <i class="bi bi-list fs-2"></i>
        </button>
      </li>
    </ul>
  </header>
</template>

<script></script>

<style lang="scss" scoped>
#headerComp {
  background: rgba(89, 109, 83, 0.2);
}
</style>