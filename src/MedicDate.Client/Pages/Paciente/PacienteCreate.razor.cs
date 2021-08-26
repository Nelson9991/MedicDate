﻿using MedicDate.Client.Data.HttpRepository.IHttpRepository;
using MedicDate.Client.Services.IServices;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using MedicDate.API.DTOs.Paciente;

namespace MedicDate.Client.Pages.Paciente
{
    public partial class PacienteCreate
    {
        [Inject] public IHttpRepository HttpRepo { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public INotificationService NotificationService { get; set; }
        [Inject] public IDialogNotificationService DialogNotificationService { get; set; }

        private readonly PacienteRequestDto _pacienteModel = new();
        private bool _isBussy;

        private async Task CreatePaciente()
        {
            _isBussy = true;

            var httpResp = await HttpRepo.Post("api/Paciente/crear", _pacienteModel);

            _isBussy = false;

            if (!httpResp.Error)
            {
                NotificationService.ShowSuccess("Operacion exitosa!", "Registro creado con éxito");

                NavigationManager.NavigateTo("pacienteList"); ;
            }
        }
    }
}