using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoAPP.Configuration;

namespace TodoAPP.Models.DTOs.Responses
{
    public class RegistationResponse : AuthResult
    {
         public bool Success { get; set; } // Garante que essa propriedade esteja definida
         public List<string> Errors { get; set; }
    }
}