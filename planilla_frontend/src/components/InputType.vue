<template>
  <div class="form-group mt-3">
    <label v-bind:for="id">{{ label }}</label>
    <input
      class="form-control"
      v-bind:type="type"
      v-bind:id="id"
      v-bind:placeholder="placeHolder"
      v-bind:minlength="min_length"
      v-bind:maxlength="max_length"
      v-bind:required="required"
      v-bind:value="modelValue"
      @input="$emit('update:modelValue', $event.target.value)"
    >
  </div>
</template>

<script>
export default {
  name:"InputType",
  props: {
    label: {
      type: String,
      required: true
    },

    id: {
      type: String,
      required: true
    },

    type: {
      type: String,
      default: "text"
    },

    placeHolder: {
      type: String,
      default: ""
    },

    min_length: {
      type: Number,
      default: 0
    },

    max_length: {
      type: Number,
      default: 40
    },

    required: {
      type: Boolean,
      default: true
    },

    modelValue: {
      type: String,
      default: ""
    }
  }
}
</script>

<style lang="scss" scoped>
</style>
