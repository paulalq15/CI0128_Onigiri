<template>
  <!--Restrict page if user is not Employer-->
  <div v-if="$session.user?.typeUser !== 'Empleador'" class="d-flex flex-column text-center">
    <h3>Acceso restringido</h3>
    <p>Esta página no está disponible</p>
  </div>

  <div class="d-flex flex-column" id="container">
    <!--Cuerpo-->
    <div class="d-flex justify-content-center align-items-center">
      <!--Formulario-->
      <form
        class="form-signig p-5 h-100 d-inline-block"
        style="max-width: 550px; width: 100%"
        @submit.prevent="saveRegisterData"
      >
        <h2 class="font-weigth-normal text-center mb-4">Registrar un empleado</h2>
        <!--Primer nombre-->
        <InputType label="Primer nombre" id="firstName" v-model="Employee.Name1" />
        <span class="text-danger" v-if="errors.Name1">{{ errors.Name1 }}</span>
        <!--Segundo nombre-->
        <InputType
          label="Segundo nombre"
          id="SecondName"
          v-model="Employee.Name2"
          :required="false"
        />
        <!--Primer apellido-->
        <InputType label="Primer apellido" id="firstSurname" v-model="Employee.Surname1" />
        <span class="text-danger" v-if="errors.Surname1">{{ errors.Surname1 }}</span>
        <!--Segundo apellido-->
        <InputType label="Segundo apellido" id="secondSurname" v-model="Employee.Surname2" />
        <span class="text-danger" v-if="errors.Surname2">{{ errors.Surname2 }}</span>
        <!--Cédula-->
        <InputType
          label="Cédula de indentidad"
          id="IDCard"
          placeHolder="0-0000-0000"
          :max_length="11"
          v-model="Employee.IdCard"
        />
        <span class="text-danger" v-if="errors.IdCard">{{ errors.IdCard }}</span>
        <!--Fecha nacimiento-->
        <InputType
          label="Fecha de nacimiento"
          id="birthday"
          type="date"
          v-model="Employee.BirthdayDate"
        />
        <span class="text-danger" v-if="errors.BirthdayDate">{{ errors.BirthdayDate }}</span>
        <!--Correo electrónico-->
        <InputType
          label="Correo electrónico"
          id="email"
          type="email"
          placeHolder="name@example.com"
          v-model="Employee.Email"
        />
        <span class="text-danger" v-if="errors.Email">{{ errors.Email }}</span>
        <!--Puesto-->
        <InputType label="Puesto" id="position" v-model="Contract.Position" />
        <span class="text-danger" v-if="errors.Position">{{ errors.Position }}</span>
        <!--Departamento-->
        <SelectType
          label="Departamento"
          id="department"
          v-model="Contract.Department"
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
        <span class="text-danger" v-if="errors.Department">{{ errors.Department }}</span>
        <!--Salario-->
        <InputType
          label="Salario"
          id="salary"
          type="number"
          v-model="Contract.Salary"
        />
        <span class="text-danger" v-if="errors.Salary">{{ errors.Salary }}</span>
        <!--Tipo de empleado-->
        <SelectType
          label="Tipo de empleado"
          id="employeeType"
          v-model="Contract.EmployeeType"
          :options="[
            { value: 'Tiempo Completo', text: 'Tiempo Completo' },
            { value: 'Medio Tiempo', text: 'Medio Tiempo' },
            { value: 'Servicios Profesionales', text: 'Servicios Profesionales' }
          ]"
        />
        <span class="text-danger" v-if="errors.EmployeeType">{{ errors.EmployeeType }}</span>
        <!--Fecha de inicio-->
        <InputType
          label="Fecha de inicio"
          id="startDate"
          type="date"
          v-model="Contract.StartDate"
        />
        <span class="text-danger" v-if="errors.StartDate">{{ errors.StartDate }}</span>
        <!--Cuenta bancaria IBAN-->
        <InputType
          label="Cuenta bancaria IBAN"
          id="iban"
          v-model="Contract.BankAccount"
        />
        <span class="text-danger" v-if="errors.BankAccount">{{ errors.BankAccount }}</span>
        <!--Botón confirmar-->
        <div class="form-group text-center">
          <LinkButton type="button" @click="saveRegisterData" text="Registrar" />
        </div>
      </form>
    </div>
  </div>
</template>

<script>
// Alerta global en App.vue
import { useGlobalAlert } from '@/utils/alerts.js';

import { toRaw } from 'vue';

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
        Email: '',
        IdCard: '',
        Name1: '',
        Name2: '',
        Surname1: '',
        Surname2: '',
        Number: '',
        BirthdayDate: '',
        TypePerson: 'Empleado',
        Status: 'Inactivo',
        Password: 'Temporary01!'
      },

      Contract: {
        Position: '',
        Department: '',
        Salary: '',
        EmployeeType: '',
        StartDate: '',
        BankAccount: '',
      },

    };
  },

  methods: {

    // Validar los datos del formulario, agregar errores al objeto errors
    async validateForm() {
      this.errors = {};

      // Nombre y apellidos
      if (!this.Persona.Name1) this.errors.Name1 = 'El primer nombre es obligatorio';
      if (!this.Persona.Surname1) this.errors.Surname1 = 'El primer apellido es obligatorio';
      if (!this.Persona.Surname2) this.errors.Surname2 = 'El segundo apellido es obligatorio';

      // Cédula
      if (!this.Persona.IdCard) this.errors.IdCard = 'La cedula es obligatorio';
      else if (!/^\d{1}-\d{4}-\d{4}$/.test(this.Persona.IdCard))
        this.errors.IdCard = 'El formato debe ser #-####-####, solo valores numéricos';

      // Teléfono
      if (!this.Persona.Number) this.errors.Number = 'El teléfono es obligatorio';
      else if (this.Persona.Number.length < 8)
        this.errors.Number = 'El teléfono debe tener al menos 8 caracteres';
      else if (!/^\d{4}-\d{4}$/.test(this.Persona.Number))
        this.errors.Number = 'El formato debe ser ####-####, solo valores numéricos';

      // Fecha de nacimiento
      if (!this.Persona.BirthdayDate)
        this.errors.BirthdayDate = 'La fecha de nacimiento es obligatoria';
      else {
        // Obtener fecha actual
        const today = new Date();
        // Convertir en objeto Date
        const birthDate = new Date(this.Persona.BirthdayDate);
        // Calcular la edad obteniendo los años
        var age = today.getFullYear() - birthDate.getFullYear();

        if (age < 18) this.errors.BirthdayDate = 'Debe ser mayor de edad para registrarse';
      }

      // Correo
      if (!this.Persona.Email) this.errors.Email = 'El correo es obligatorio';
      else if (!/\S+@\S+\.\S+/.test(this.Persona.Email)) this.errors.Email = 'Correo inválido';
      else {
        // Esperar la verificación de correo
        try {
          const response = await URLBaseAPI.get('/api/PersonUser/emailCheck', {
            params: { email: this.Persona.Email },
          });
          if (response.data > 0) {
            this.errors.Email = 'El correo ya está en uso';
          }
        } catch (error) {
          console.error(error);
        }
      }
      // Si no hay errores, el formulario es válido
      return Object.keys(this.errors).length === 0;
    },

    // Enviar datos del formulario para guardar
    async saveRegisterData() {
      if (!(await this.validateForm())) {
        return;
      }

      const register = {
        personData: toRaw(this.Employee),
        //password: this.password,
        //otherSigns: this.Direccion.otherSigns,
        //zipCode: this.zipCode.value,
      };

      console.log(register);

      URLBaseAPI.post('/api/PersonUser/register', register)
        .then((response) => {
          console.log('OK:', response.data);

          // Uso de alerta global para notificar un registro exitoso
          const alert = useGlobalAlert();

          alert.show(
            'Registro exitoso. Por favor, active su cuenta mediante el correo de activación enviado a ' +
              this.Persona.Email +
              ' para poder iniciar sesión',
            'success'
          );

          // Redireccionamiento que permite mantener la alerta
          this.$router.push('/');
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