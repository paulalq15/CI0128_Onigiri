<template>
  <div class="text-center">
    <div class="ratio ratio-1x1 mx-auto" style="width: 8rem">
      <RouterLink
        class="p-2 align-items-center justify-content-center border rounded bg-light d-flex flex-column text-dark text-decoration-none"
        :to="to"
      >
        <i :class="icon" class="fs-1"></i>
      </RouterLink>
    </div>
    <div class="mt-2 fw-semibold">{{ title }}</div>
  </div>
</template>

<script>
export default {
  name: 'HomePageCard',
  props: {
    title: { type: String, required: true },
    icon: { type: String, required: true },
    to: { type: String, required: true },
  },
};
</script>

<style lang="scss" scoped></style>