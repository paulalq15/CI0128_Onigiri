<template>
  <div id="reportFilters">
    <div>
      <label for="selectPeriod" class="form-label fw-bold">Periodo</label>
      <select id="selectPeriod" class="form-select" v-model="selectedPayrollId">
        <option v-for="payroll in payrollList" :key="payroll.payrollId" :value="payroll.payrollId">
          {{ payroll.periodLabel }}
        </option>
      </select>
    </div>
  </div>

  <div id="buttons">
    <LinkButton text="Descargar PDF" @click="downloadPDF()" />
  </div>

  <div id="reportContent">
    <div v-if="isLoading" class="text-muted">Cargando reporte</div>

    <div v-else-if="reportResult" ref="reportContainer">
      <h4>Reporte detalle de pago de planilla</h4>

      <p><strong>Empresa:</strong> {{ reportResult.reportInfo.CompanyName }}</p>
      <p><strong>Empleado:</strong> {{ reportResult.reportInfo.EmployeeName }}</p>
      <p><strong>Tipo de contrato:</strong> {{ reportResult.reportInfo.EmployeeType }}</p>
      <p><strong>Fecha de pago:</strong> {{ formatDate(reportResult.reportInfo.PaymentDate) }}</p>

      <div id="reportTable" class="table-responsive">
        <table class="table">
          <tbody>
            <tr v-for="(row, index) in reportResult.rows" :key="index" :class="lineClasses(row)">
              <td>{{ row['Descripción'] }}</td>
              <td class="amount">{{ fmtCRC(row['Monto']) }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div v-else class="text-muted">No hay datos para mostrar.</div>
  </div>
</template>

<script>
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import URLBaseAPI from '../../axiosAPIInstances.js';
import LinkButton from '../LinkButton.vue';
import { useGlobalAlert } from '@/utils/alerts.js';

export default {
  components: {
    LinkButton,
  },
  data() {
    return {
      payrollList: [],
      selectedPayrollId: null,
      reportResult: null,
      isLoading: false,
    };
  },
  methods: {
    getPayrollList() {
      const companyId = this.$session.user?.companyUniqueId;
      const employeeId = Number(this.$session.user?.personId);

      URLBaseAPI.get('/api/Reports/employee/payroll-periods', {
        params: {
          companyId: companyId,
          employeeId: employeeId,
        },
      })
        .then((response) => {
          this.payrollList = response.data || [];
          if (this.payrollList.length > 0) {
            this.selectedPayrollId = this.payrollList[0].payrollId;
          }
        })
        .catch((error) => {
          const data = error && error.response && error.response.data ? error.response.data : null;
          const msg =
            typeof data === 'string'
              ? data
              : (data && (data.message || data.detail)) || 'Error cargando las últimas planillas';

          const alert = useGlobalAlert();
          alert.show(msg, 'warning');
        });
    },
    loadReport() {
      if (!this.selectedPayrollId) return;

      const companyId = this.$session.user?.companyUniqueId;
      const employeeId = Number(this.$session.user?.personId);

      const params = {
        reportCode: 'EmployeeDetailPayroll',
        companyId,
        employeeId,
        payrollId: this.selectedPayrollId,
      };

      this.isLoading = true;
      URLBaseAPI.post('/api/Reports/data', params)
        .then((response) => {
          this.reportResult = response.data;
        })
        .catch((error) => {
          const data = error && error.response && error.response.data ? error.response.data : null;
          const msg =
            typeof data === 'string'
              ? data
              : (data && (data.message || data.detail)) || 'Error cargando el detalle de planilla';

          const alert = useGlobalAlert();
          alert.show(msg, 'warning');
          this.reportResult = null;
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    fmtCRC(v) {
      return new Intl.NumberFormat('es-CR', {
        style: 'currency',
        currency: 'CRC',
        currencyDisplay: 'symbol',
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      }).format(Number(v ?? 0));
    },
    formatDate(dateString) {
      if (!dateString) return '';
      const date = new Date(dateString);
      return isNaN(date)
        ? ''
        : new Intl.DateTimeFormat('es-CR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
          }).format(date);
    },
    lineClasses(row) {
      const desc = (row['Descripción'] || '').toString().toLowerCase();
      return {
        'line-gross': desc === 'salario bruto',
        'line-total': desc.includes('total'),
        'line-net': desc === 'pago neto',
      };
    },
    async downloadPDF() {
      const element = this.$refs.reportContainer;
      if (!element || !this.reportResult) return;

      const payment = this.reportResult.reportInfo?.PaymentDate;
      let fileDate = '';

      if (payment) {
        const d = new Date(payment);
        if (!isNaN(d)) {
          const yyyy = d.getFullYear();
          const mm = String(d.getMonth() + 1).padStart(2, '0');
          const dd = String(d.getDate()).padStart(2, '0');
          fileDate = `${yyyy}${mm}${dd}`;
        }
      }
      const fileName = fileDate ? `Detalle_planilla_${fileDate}.pdf` : 'Detalle_planilla.pdf';
      
      const canvas = await html2canvas(element, {
        scale: 2,
        useCORS: true,
        backgroundColor: '#ffffff',
      });

      const imgData = canvas.toDataURL('image/png');
      const pdf = new jsPDF('p', 'mm', 'a4');

      const pageWidth  = pdf.internal.pageSize.getWidth();
      const pageHeight = pdf.internal.pageSize.getHeight();
      
      const sideMargin = 10;
      const topAfterTitle = 25;

      const imgWidth = pageWidth - sideMargin * 2;
      const imgHeight = (canvas.height * imgWidth) / canvas.width;

      let finalWidth = imgWidth;
      let finalHeight = imgHeight;

      const availableHeight = pageHeight - topAfterTitle - sideMargin;
      if (imgHeight > availableHeight) {
        const ratio = availableHeight / imgHeight;
        finalWidth = imgWidth * ratio;
        finalHeight = imgHeight * ratio;
      }

      const x = (pageWidth - finalWidth) / 2;
      const y = topAfterTitle;

      pdf.addImage(imgData, 'PNG', x, y, finalWidth, finalHeight);
      pdf.save(fileName);
    },
  },
  watch: {
    selectedPayrollId() {
      this.loadReport();
    },
  },
  mounted() {
    this.getPayrollList();
  },
};
</script>

<style lang="scss" scoped>
  #reportFilters {
    display: flex;
    gap: 20px;
    margin-bottom: 20px;
  }

  #reportFilters > div {
    width: 50%;
    text-align: left;
  }

  #buttons {
    display: inline-block;
    margin-bottom: 20px;
  }

  #reportContent {
    background: white;
    border-radius: 10px;

    display: flex;
    flex-direction: column;
    align-items: flex-start;

    padding: 20px;

    width: 100%;
    align-self: stretch;
    text-align: left;
  }

  #reportContent > div[ref="reportContainer"], #reportContent > div {
    width: 100%;
  }

  #reportTable .amount {
    min-width: 180px;
    text-align: right;
  }

  #reportTable tr.line-gross td,
  #reportTable tr.line-total td {
    font-weight: 600;
  }

  #reportTable tr.line-net td {
    font-weight: 700;
    font-size: 1.05rem;
    padding-top: 10px;
  }
</style>