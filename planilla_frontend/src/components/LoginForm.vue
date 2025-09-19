<template>
  <div class="page d-flex flex-column min-vh-100">
    <HeaderOnigiri />

    <div class="flex-fill d-flex justify-content-center align-items-center">
      <div style="max-width: 400px; width: 100%;">
        <div class="text-center mb-3">
          <h3 class="mb-3">Iniciar sesión</h3>
        </div>

        <form @submit.prevent="onSubmit" class="needs-validation" novalidate>
          <!-- Email -->
          <div class="mb-3">
            <label for="email" class="form-label required">Correo electrónico</label>
            <input
              id="email"
              type="email"
              class="form-control"
              v-model="email"
              required
              placeholder="tu@correo.com"
            />
            <div class="invalid-feedback">Ingrese un correo válido</div>
          </div>

          <!-- Password -->
          <div class="mb-3">
            <label for="password" class="form-label required">Contraseña</label>
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
            <div class="invalid-feedback">Ingrese su contraseña</div>
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
            <button type="submit" class="btn btn-custom">Ingresar</button>
          </div>
        </form>
      </div>
    </div>
    <FooterComp />
  </div>
</template>

<script>
import { ref, onMounted } from 'vue';
import FooterComp from './FooterComp.vue';
import HeaderOnigiri from './HeaderOnigiri.vue';

export default {
  components: { HeaderOnigiri, FooterComp },
  setup() {
    const email = ref('');
    const password = ref('');
    const showPassword = ref(false);

    const showToast = ref(false);
    const toastMessage = ref('');
    const toastType = ref('bg-success');

    const initBootstrapValidation = () => {
      const forms = document.querySelectorAll('.needs-validation');
      Array.from(forms).forEach((form) => {
        form.addEventListener(
          'submit',
          (event) => {
            if (!form.checkValidity()) {
              event.preventDefault();
              event.stopPropagation();
            }
            form.classList.add('was-validated');
          },
          false
        );
      });
    };

    const onSubmit = () => {
      if (!email.value || !password.value) return;

      toastMessage.value = `Bienvenido, ${email.value}`;
      toastType.value = 'bg-success';
      showToast.value = true;
      setTimeout(() => (showToast.value = false), 3000);
    };

    onMounted(() => {
      initBootstrapValidation();
    });

    return {
      email,
      password,
      showPassword,
      showToast,
      toastMessage,
      toastType,
      onSubmit,
    };
  },
};
</script>

<style></style>
