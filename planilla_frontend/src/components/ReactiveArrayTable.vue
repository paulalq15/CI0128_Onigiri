<template>
  <div class="table-responsive pt-3 pb-3 ps-4 pe-4" style="background-color: white; border-radius: 10px;">
    <table class="table">
      <thead>
        <tr>
          <th scope="col">#</th>
          <th v-for="(head, index) in tableHeader" :key="index" scope="col">
            {{ head.label }}
          </th>
        </tr>
      </thead>
      <tbody>
        <template v-for="(element, index) in tableElements" :key="index">
          <tr>
            <td>{{ index + 1 }}</td>
            <td v-for="(head, j) in tableHeader" :key="j">
              {{ element[head.key] }}
            </td>
          </tr>
          <tr v-if="index < tableElements.length - 1" class="spacer">
            <td :colspan="tableHeader.length + 1"></td>
          </tr>
        </template>
      </tbody>
    </table>
  </div>
</template>

<script>
export default {
  name: "ReactiveObjectTable",
  props: {
    tableHeader: { type: Array, required: true },
    tableElements: { type: Array, required: true }
  },
};
</script>

<style scoped>
  table.table td {
    border: none;
  }
</style>
