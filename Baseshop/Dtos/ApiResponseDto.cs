using System.ComponentModel;
using Baseshop.Models;
using System.ComponentModel.DataAnnotations;
using Baseshop.ValidationAttributes;

namespace Baseshop.Dtos
{
    public class ApiResponseDto
    {
        public object Data { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }


    }
}
