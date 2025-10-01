<template>
  <!--Restrict page if user is not Employer-->
  <div v-if="$session.user?.typeUser !== 'Empleador'" class="d-flex flex-column text-center">
    <h3>Acceso restringido</h3>
    <p>Esta página no está disponible</p>
  </div>

  <div v-else class="d-flex flex-column" id="container">
    <!--Cuerpo-->
    <div class="d-flex justify-content-center align-items-center">
      <!--Formulario-->
      <form
        class="form-signig p-5 h-100 d-inline-block"
        style="max-width: 550px; width: 100%"
        @submit.prevent="saveEmployee"
      >
        <h2 class="font-weigth-normal text-center mb-4">Registrar un empleado</h2>
        <!--Primer nombre-->
        <InputType label="Primer nombre" id="firstName" v-model="Employee.name1" />
        <span class="text-danger" v-if="errors.name1">{{ errors.name1 }}</span>
        <!--Segundo nombre-->
        <InputType
          label="Segundo nombre"
          id="SecondName"
          v-model="Employee.name2"
          :required="false"
        />
        <!--Primer apellido-->
        <InputType label="Primer apellido" id="firstSurname" v-model="Employee.surname1" />
        <span class="text-danger" v-if="errors.surname1">{{ errors.surname1 }}</span>
        <!--Segundo apellido-->
        <InputType label="Segundo apellido" id="secondSurname" v-model="Employee.surname2" />
        <span class="text-danger" v-if="errors.surname2">{{ errors.surname2 }}</span>
        <!--Cédula-->
        <InputType
          label="Cédula de indentidad"
          id="IDCard"
          placeHolder="0-0000-0000"
          :max_length="11"
          v-model="Employee.idCard"
        />
        <span class="text-danger" v-if="errors.idCard">{{ errors.idCard }}</span>
        <!--Fecha nacimiento-->
        <InputType
          label="Fecha de nacimiento"
          id="birthday"
          type="date"
          v-model="Employee.birthDate"
        />
        <span class="text-danger" v-if="errors.birthDate">{{ errors.birthDate }}</span>
        <!--Correo electrónico-->
        <InputType
          label="Correo electrónico"
          id="email"
          type="email"
          placeHolder="name@example.com"
          v-model="Employee.email"
        />
        <span class="text-danger" v-if="errors.email">{{ errors.email }}</span>
        <!--Puesto-->
        <InputType label="Puesto" id="position" v-model="Contract.position" />
        <span class="text-danger" v-if="errors.position">{{ errors.position }}</span>
        <!--Departamento-->
        <SelectType
          label="Departamento"
          id="department"
          v-model="Contract.department"
          :options="[
            { value: 'Administracion', text: 'Administración' },
            { value: 'Contabilidad', text: 'Contabilidad' },
            { value: 'Mercadeo', text: 'Mercadeo' },
            { value: 'Operaciones', text: 'Operaciones' },
            { value: 'Produccion', text: 'Producción' },
            { value: 'Recursos Humanos', text: 'Recursos Humanos' },
            { value: 'Ventas', text: 'Ventas' }
          ]"
        />
        <span class="text-danger" v-if="errors.department">{{ errors.department }}</span>
        <!--Salario-->
        <InputType
          label="Salario"
          id="salary"
          type="number"
          v-model="Contract.salary"
        />
        <span class="text-danger" v-if="errors.salary">{{ errors.salary }}</span>
        <!--Tipo de empleado-->
        <SelectType
          label="Tipo de empleado"
          id="employeeType"
          v-model="Contract.employeeType"
          :options="[
            { value: 'Tiempo Completo', text: 'Tiempo Completo' },
            { value: 'Medio Tiempo', text: 'Medio Tiempo' },
            { value: 'Servicios Profesionales', text: 'Servicios Profesionales' }
          ]"
        />
        <span class="text-danger" v-if="errors.employeeType">{{ errors.employeeType }}</span>
        <!--Fecha de inicio-->
        <InputType
          label="Fecha de inicio"
          id="startDate"
          type="date"
          v-model="Contract.startDate"
        />
        <span class="text-danger" v-if="errors.startDate">{{ errors.startDate }}</span>
        <!--Cuenta bancaria IBAN-->
        <InputType
          label="Cuenta bancaria IBAN"
          id="iban"
          v-model="Contract.bankAccount"
        />
        <span class="text-danger" v-if="errors.bankAccount">{{ errors.bankAccount }}</span>
        <!--Botón confirmar-->
        <div class="form-group text-center">
          <LinkButton type="button" @click="saveEmployee" text="Guardar Empleado" />
        </div>
      </form>
    </div>
  </div>
</template>

<script>
// Alerta global en App.vue
import { useGlobalAlert } from '@/utils/alerts.js';

// Base URL de la API
import URLBaseAPI from '../axiosAPIInstances.js';

import LinkButton from './LinkButton.vue';
import InputType from './InputType.vue';
import SelectType from './SelectType.vue';

export default {
  components: {
    InputType,
    LinkButton,
    SelectType,
  },

  data() {
    return {
      // Guarda errores de validación
      errors: {},

      Employee: {
        idCard: '',
        name1: '',
        name2: '',
        surname1: '',
        surname2: '',
        phone: '9999-9999',
        birthDate: '',
        personType: 'Empleado',
        email: '',
        password: 'Temporary01!',
        status: 'Inactivo',
        companyUniqueId: ''
      },

      Contract: {
        position: '',
        department: '',
        salary: '',
        employeeType: '',
        startDate: '',
        bankAccount: '',
      },
    };
  },

  methods: {
    // Validar los datos del formulario, agregar errores al objeto errors
    async validateForm() {
      this.errors = {};

      // Nombre y apellidos
      if (!this.Employee.name1) this.errors.name1 = 'El primer nombre es obligatorio';
      if (!this.Employee.surname1) this.errors.surname1 = 'El primer apellido es obligatorio';
      if (!this.Employee.surname2) this.errors.surname2 = 'El segundo apellido es obligatorio';

      // Cédula
      if (!this.Employee.idCard) this.errors.idCard = 'La cedula es obligatorio';
      else if (!/^\d{1}-\d{4}-\d{4}$/.test(this.Employee.idCard))
        this.errors.idCard = 'El formato debe ser #-####-####, solo valores numéricos';

      // Fecha de nacimiento
      if (!this.Employee.birthDate)
        this.errors.birthDate = 'La fecha de nacimiento es obligatoria';
      else {
        // Obtener fecha actual
        const today = new Date();
        // Convertir en objeto Date
        const birthDate = new Date(this.Employee.birthDate);
        // Calcular la edad obteniendo los años
        var age = today.getFullYear() - birthDate.getFullYear();

        if (age < 18) this.errors.birthDate = 'Debe ser mayor de edad para registrarse';
      }

      // Correo
      if (!this.Employee.email) this.errors.email = 'El correo es obligatorio';
      else if (!/\S+@\S+\.\S+/.test(this.Employee.email)) this.errors.email = 'Correo inválido';
      else {
        // Esperar la verificación de correo
        try {
          const response = await URLBaseAPI.get('/api/PersonUser/emailCheck', {
            params: { email: this.Employee.email },
          });
          if (response.data > 0) {
            this.errors.email = 'El correo ya está en uso';
          }
        } catch (error) {
          console.error(error);
        }
      }

      // Salario
      if(!this.Contract.salary) this.errors.salary = 'El salario es obligatorio';
      else {
        if (this.Contract.salary < '0') this.errors.salary = 'El salario debe ser mayor a 0'
      }

      //Fecha Inicio
      if(!this.Contract.startDate) this.errors.startDate = 'La fecha de inicio es obligatorio';

      //IBAN
      if(!this.Contract.bankAccount) this.errors.bankAccount = 'La cuenta bancaria es obligatorio';
      else if (this.Contract.bankAccount.length !== 22)
        this.errors.bankAccount = 'Tamaño de cuenta incorrecto';
      else {
        if (!/^CR\d{20}$/.test(this.Contract.bankAccount))
          this.errors.bankAccount = 'Formato IBAN de Costa Rica inválido (CR + 20 dígitos)';
      }

      // Si no hay errores, el formulario es válido
      return Object.keys(this.errors).length === 0;
    },

    // Enviar datos del formulario para guardar
    async saveEmployee() {
      if (!(await this.validateForm())) {
        return;
      }

      const payload = {
        personData: {
          cedula: this.Employee.idCard,
          name1: this.Employee.name1,
          name2: this.Employee.name2 || null,
          surname1: this.Employee.surname1,
          surname2: this.Employee.surname2 || '',
          phone: this.Employee.phone,
          birthDate: this.Employee.birthDate,
          personType: this.Employee.personType,
        },
        userData: {
          email: this.Employee.email,
          password: this.Employee.password,
          status: this.Employee.status
        },
        contractData: {
          position: this.Contract.position,
          department: this.Contract.department,
          salary: Number(this.Contract.salary),
          employeeType: this.Contract.employeeType,
          startDate: this.Contract.startDate,
          bankAccount: this.Contract.bankAccount
        },
        companyUniqueId: Number(this.$session.user?.companyUniqueId)
      };

      URLBaseAPI.post('/api/Employee', payload)
        .then((response) => {
          console.log('OK:', response.data);

          // Uso de alerta global para notificar un registro exitoso
          const alert = useGlobalAlert();

          alert.show(
            'Registro exitoso.',
            'success'
          );

          // Redireccionamiento que permite mantener la alerta
          this.$router.push('/home');
        })
        .catch((error) => {
          if (error.response) console.log('Error del backend:', error.response.data);
          else console.log('Error de red:', error.message);
        });
    },
  },

  mounted() {
  },
};
</script>

<style lang="scss" scoped>
  .form-group {
    margin-bottom: 20px;
  }

  .text-danger {
    font-weight: bold;
    font-size: medium;
  }

  #termsandlogin a {
    color: blue;
  }
</style>