<template>
  <div class="container min-vh-100 d-flex flex-column justify-content-center align-items-center bg-light">
    <div class="text-center mb-4">
      <img src="@/assets/OnigiriLogo1.png" alt="Logo Onigiri" width="100" class="mb-2" />
    </div>

    <div class="card shadow-sm p-4" style="max-width: 400px; width: 100%;">
      <h3 class="text-center mb-3">Iniciar sesión</h3>

      <form @submit.prevent="onSubmit">
        <!-- Email -->
        <div class="mb-3">
          <label for="email" class="form-label">Correo electrónico</label>
          <input
            id="email"
            type="email"
            class="form-control"
            v-model="email"
            required
            placeholder="tu@correo.com"
          />
        </div>

        <!-- Password -->
        <div class="mb-3">
          <label for="password" class="form-label">Contraseña</label>
          <div class="input-group">
            <input
              id="password"
              :type="showPassword ? 'text' : 'password'"
              class="form-control"
              v-model="password"
              required
              placeholder="••••••••"
            />
            <button
              type="button"
              class="btn btn-outline-secondary"
              @click="showPassword = !showPassword"
            >
              <i :class="showPassword ? 'bi bi-eye-slash' : 'bi bi-eye'"></i>
            </button>
          </div>
        </div>

        <!-- Links -->
        <div class="d-flex flex-column text-center mb-3">
          <small>
            ¿No tienes una cuenta aún?
            <a href="#" class="text-decoration-none">Regístrate como empleador</a>
          </small>
          <small>
            <a href="#" class="text-decoration-none">¿Has olvidado tu contraseña?</a>
          </small>
        </div>

        <!-- Submit -->
        <div class="d-grid">
          <button type="submit" class="btn btn-success">Ingresar</button>
        </div>
      </form>

      <p class="text-center mt-3 mb-0 text-muted small">
        © 2025 Todos los derechos reservados - Onigiri Intelligent Solutions
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const email = ref('');
const password = ref('');
const showPassword = ref(false);

function onSubmit() {
  alert(`Email: ${email.value}\nPassword: ${password.value}`);
}
</script>

<!-- Agregar Bootstrap y Bootstrap Icons en index.html -->
<!-- 
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
<link href="https://cdn.jsdelivr.net/npm/bootstrap-icons/font/bootstrap-icons.css" rel="stylesheet">
-->
