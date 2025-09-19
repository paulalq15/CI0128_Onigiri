<template>
  <div id="app">
    <LoginForm />
  </div>
</template>

<script>
import LoginForm from './components/LoginForm.vue'

export default {
  name: 'App',
  components: {
    LoginForm
  }
}
</script>

<style lang="scss">
  body {
    background: #596d53;
    background: linear-gradient(
      357deg,
      rgba(89, 109, 83, 1) 0%,
      rgba(225, 245, 219, 1) 80%
    );
    min-height: 100vh;
  }

  /* Etiquetas con * rojo */
  .form-label.required::after {
    content: " *";
    color: red;
  }

  /* Botón personalizado */
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