<template>
  <div class="container p-5 pt-4 pb-4 mt-5" style="background-color: white; border-radius: 20px;">
    <h3 class="display-5 text-center mb-4">{{ title }}</h3>

    <table class="table table-striped">
      <thead>
        <tr>
          <th scope="col">#</th>
          <th v-for="(head, index) in tableHeader" :key="index" scope="col">
            {{ head.label }}
          </th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(element, index) in tableElements" :key="index">
          <th scope="row">{{ index + 1 }}</th>
          <td v-for="(head, j) in tableHeader" :key="j">
            {{ element[head.key] }}
          </td>
        </tr>
      </tbody>
    </table>

  </div>
</template>

<script>
export default {
  name: "ReactiveObjectTable",
  props: {
    title: {
      type: String,
      default: "Tabla"
    },
    tableHeader: {
      type: Array,
      required: true
    },
    tableElements: {
      type: Array,
      required: true
    }
  }
};
</script>

<style scoped>

  table.table td {
    border: none;
  }
</style>
