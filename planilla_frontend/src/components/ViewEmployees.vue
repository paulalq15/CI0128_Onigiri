<template>
  <div class="d-flex flex-column">
    <div class="container mt-5">
      <h1 class="display-4 text-center">Lista de Empleados</h1>

      <table class="table is-bordered is-striped is-narrow is-hoverable is-fullwidth">
        <thead>
          <tr>
            <th>Cédula</th>
            <th>Primer Nombre</th>
            <th>Segundo Nombre</th>
            <th>Primer Apellido</th>
            <th>Segundo Apellido</th>
            <th>Fecha Nacimiento</th>
            <th>Email</th>
            <th>Tipo Contrato</th>
            <th>Puesto</th>
            <th>Departamento</th>
            <th>Acciones</th>
            <th></th>
          </tr>
        </thead>

        <tbody>
          <tr v-for="(employee, index) of employees" :key="index">
            <td>{{ employee.idCard }}</td>
            <td>{{ employee.name1 }}</td>
            <td>{{ employee.name2 }}</td>
            <td>{{ employee.surname1 }}</td>
            <td>{{ employee.surname2 }}</td>
            <td>{{ formatDate(employee.birthdayDate) }}</td>
            <td>{{ employee.email }}</td>
            <td>{{ employee.contractType }}</td>
            <td>{{ employee.jobPosition }}</td>
            <td>{{ employee.department }}</td>
            <td>
              <button class="btn btn-sm btn-primary" @click="goEdit(employee)">
                Editar
              </button>
            </td>
            <td>
              <button class="btn btn-sm btn-danger" @click="deleteEmployee(employee)">
                Eliminar
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import URLBaseAPI from '../axiosAPIInstances.js';
import { getUser } from '../session.js';
// import PopUp from './alerts/PopUp.vue';

const router = useRouter();
const employees = ref([]);
const user = ref(null);

function formatDate(d) { return new Date(d).toLocaleDateString(); }

async function getEmployees() {
  const u = getUser();

  if (!u) return;

  user.value = u;

  try {
    const { data } = await URLBaseAPI.get('/api/PersonUser/getEmployeesByCompanyId', {
      params: { companyId: u.companyUniqueId }
    });
    employees.value = data;
  } catch (e) {
    console.error('Error al cargar empleados:', e);
    employees.value = [];
  }
}

function goEdit(emp) {
  const personId = emp.idPerson
  if (!personId && personId !== 0) {
    console.warn('No se encontró personId', emp);
    return;
  }

  router.push({ name: 'ModifyEmployees', query: { personId } });
}

async function deleteEmployee(employee) {
  if (confirm("¿Seguro que desea eliminar al empleado " + employee.name1 + " " + employee.surname1 + "?")) {
    try {
      await URLBaseAPI.delete(`/api/Employee/${employee.idUser}`);
      alert('¡Empleado eliminada exitosamente!');
      setTimeout(() => window.location.reload(), 500);
    }

    catch (error) {
      console.error('Error: ', error);
    }
  }

  else {
    return;
  }
}

onMounted(getEmployees);

</script>
