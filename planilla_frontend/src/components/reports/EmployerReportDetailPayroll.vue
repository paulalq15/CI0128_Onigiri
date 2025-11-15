<template>
  <div id="report" ref="reportContainer">
    Reporte detallado del pago de planilla (Empleador, PLAN-110)
  </div>

  <button class="btn btn-primary" @click="exportToPDF">Exportar a PDF</button>
</template>

<script>
  import jsPDF from "jspdf";

  export default {
    data() {
      return {
      };
    },

    methods: {
      exportToPDF() {
        const doc = new jsPDF();
        const element = this.$refs.reportContainer;
      
        doc.html(element, {
          callback: function (pdf) {
            pdf.save("EmployerReportDetailPayroll.pdf");
          },

          x: 10,
          y: 10,
          width: 190,
          windowWidth: element.scrollWidth,
        });
      }
    }
  }

</script>

<style lang="scss" scoped>

</style>
