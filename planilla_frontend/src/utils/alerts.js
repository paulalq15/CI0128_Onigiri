// PopUp
import Swal from 'sweetalert2'

// Pinia nos permite crear un tipo de alerta global
import { defineStore } from 'pinia'


export const useGlobalAlert = defineStore('globalAlert', {
  state: () => ({ message: null, type: null }),
  actions: {
    show(message, type = "success") {
      this.message = message
      this.type = type
    },
    
    clear() {
      this.message = null
      this.type = null
    }
  }
})

export const popUpAlert = defineStore('popUpAlert', {
  actions: {
    async confirmAlert(
        message = "¿Está seguro de que desea eliminar este elemento?",
        confirmText = "Sí, eliminar",
        cancelText = "Cancelar") {
      const result = await Swal.fire({
        title: message,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: confirmText,
        cancelButtonText: cancelText,
      });

      return result.isConfirmed;
    }
  }
})
