<template>
  <div class="form-group mt-3">
    <label :for="id">
      {{ label }}
      <span v-if="required" style="color: red;"> *</span>
    </label>
    <select
      class="form-select"
      :id="id"
      :required="required"
      :value="modelValue"
      @change="$emit('update:modelValue', $event.target.value)"
    >
      <option value="" disabled>Seleccione...</option>
      <option
        v-for="(option, index) in options"
        :key="index"
        :value="option.value"
      >
        {{ option.text }}
      </option>
    </select>
  </div>
</template>

<script>
export default {
  name: "SelectType",
  props: {
    label: {
      type: String,
      required: true,
    },
    id: {
      type: String,
      required: true,
    },
    required: {
      type: Boolean,
      default: true,
    },
    modelValue: {
      type: String,
      default: "",
    },
    options: {
      type: Array,
      required: true,
      // ejemplo: [{ value: "Tiempo Completo", text: "Tiempo Completo" }]
    },
  },
};
</script>

<style lang="scss" scoped>
</style>
