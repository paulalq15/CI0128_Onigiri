<template>
  <div class="container py-4">
    <h2 class="mb-3">Welcome, {{ displayName }}</h2>
    <p class="text-muted">Sesión iniciada como {{ user?.correo }}</p>

    <button class="btn btn-outline-danger" @click="logout">Cerrar sesión</button>
  </div>
</template>

<script>
import { getUser, clearUser } from "../session";

export default {
  name: "HomePage",
  data() {
    return {
      user: null,
    };
  },
  computed: {
    displayName() {
      if (!this.user) return "";
      return this.user.fullName?.trim() || this.user.email || "Usuario";
    },
  },
  created() {
    this.user = getUser();
  },
  methods: {
    logout() {
      clearUser();
      this.$router.push({ name: "Login" });
    },
  },
};
</script>
