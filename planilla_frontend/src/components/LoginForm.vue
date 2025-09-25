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

          <!-- Alerts -->
          <div aria-live="polite" aria-atomic="true" class="mb-3">
            <div v-if="errorMsg" class="alert alert-danger py-2 mb-2">
              {{ errorMsg }}
            </div>
            <div v-if="successMsg" class="alert alert-success py-2 mb-0">
              {{ successMsg }}
            </div>
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
import HeaderOnigiri from "./HeaderOnigiri.vue";
import FooterComp from "./FooterComp.vue";
import axios from "axios";
import { setUser } from "../session";

export default {
  name: "LoginForm",
  components: { HeaderOnigiri, FooterComp },
  data() {
    return {
      email: "",
      password: "",
      showPassword: false,
      loading: false,
      errorMsg: "",
      successMsg: "",
    };
  },
  methods: {
    async onSubmit() {
      this.errorMsg = "";
      this.successMsg = "";

      if (!this.email || !this.password) {
        this.errorMsg = "Ingrese correo y contraseña.";
        return;
      }

      this.loading = true;
      try {
        const { data } = await axios.post("https://localhost:7115/api/Auth/login", {
          email: this.email.trim(),
          password: this.password,
        });
        console.log("[Login] Respuesta:", data);

        if (data?.success) {
          setUser({
            userId: data.UserId,
            personId: data.PersonID,
            fullName: data.FullName,
            typeUser: data.PersonType,
            email: data.Email,
            companyUniqueId: data.CompanyUniqueId
          });

          this.successMsg = "Login exitoso. Redirigiendo…";
          this.$router.push({ name: "Home Page" });
        } else {
          this.errorMsg = data?.message || "No se pudo iniciar sesión.";
        }
      } catch (err) {
        if (err?.response?.data?.message) {
          this.errorMsg = err.response.data.message;
        } else if (err?.message?.includes("Network Error")) {
          this.errorMsg = "No hay conexión con el servidor. Verifique la URL/puerto.";
        } else {
          this.errorMsg = "Error al intentar iniciar sesión.";
        }
      } finally {
        this.loading = false;
      }
    },
  },
};
</script>

<style lang="scss" scoped>
    .form-label.required::after {
        content: " *";
        color: red;
    }

    .btn-custom {
        background-color: #234d34;
        color: #fff;
        border: none;
        border-radius: 0.375rem;
        padding: 0.5rem 1rem;
        font-weight: 600;
        text-align: center;
        }

        .btn-custom:hover {
        background-color: #1b3d2a;
        color: #fff;
        }

</style>