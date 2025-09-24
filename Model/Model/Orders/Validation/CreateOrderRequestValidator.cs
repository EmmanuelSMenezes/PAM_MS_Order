using Domain.Model.Request;
using FluentValidation;

namespace Domain.Model
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator()
        {
            RuleFor(s => s.Shipping_options.Value)
              .NotEmpty().WithMessage("Valor do frete é obrigatório.")
              .NotNull().WithMessage("Valor do frete é obrigatório.");
            RuleFor(s => s.Amount)
              .NotEmpty().WithMessage("Valor total é obrigatório.")
              .NotNull().WithMessage("Valor total é obrigatório.");
            RuleFor(s => s.Change)
              .NotEmpty().WithMessage("Troco é obrigatório.")
              .NotNull().WithMessage("Troco é obrigatório.");
            RuleFor(s => s.Consumer_id)
              .NotEmpty().WithMessage("Consumidor é obrigatório.")
              .NotNull().WithMessage("Consumidor é obrigatório.");
            RuleFor(s => s.Order_itens)
             .NotEmpty().WithMessage("Item é obrigatório.")
             .NotNull().WithMessage("Item é obrigatório.")
             .Must(s => s.Count > 0).WithMessage("Item é obrigatório.");
            RuleFor(s => s.Address.Legal_name)
            .NotEmpty().WithMessage("Nome do consumidor é obrigatório.")
            .NotNull().WithMessage("Nome do consumidor é obrigatório.");
            RuleFor(s => s.Address.Fantasy_name)
            .NotEmpty().WithMessage("Nome fantasia do consumidor é obrigatório.")
            .NotNull().WithMessage("Nome fantasia do consumidor é obrigatório.");
            RuleFor(s => s.Address.Email)
            .NotEmpty().WithMessage("Email do consumidor é obrigatório.")
            .NotNull().WithMessage("Email do consumidor é obrigatório.");
            RuleFor(s => s.Address.Phone_number)
            .NotEmpty().WithMessage("Telefone do consumidor é obrigatório.")
            .NotNull().WithMessage("Telefone do consumidor é obrigatório.");
            RuleFor(s => s.Address.Document)
            .NotEmpty().WithMessage("Documento do consumidor é obrigatório.")
            .NotNull().WithMessage("Documento do consumidor é obrigatório.");
            RuleFor(s => s.Address.Phone_number)
            .NotEmpty().WithMessage("Telefone do consumidor é obrigatório.")
            .NotNull().WithMessage("Telefone do consumidor é obrigatório.");
            RuleFor(s => s.Address.Street)
            .NotEmpty().WithMessage("Rua do consumidor é obrigatório.")
            .NotNull().WithMessage("Rua do consumidor é obrigatório.");
            RuleFor(s => s.Address.City)
            .NotEmpty().WithMessage("Cidade do consumidor é obrigatório.")
            .NotNull().WithMessage("Cidade do consumidor é obrigatório.");
            RuleFor(s => s.Address.State)
            .NotEmpty().WithMessage("Estado do consumidor é obrigatório.")
            .NotNull().WithMessage("Estado do consumidor é obrigatório.");
            RuleFor(s => s.Address.Number)
            .NotEmpty().WithMessage("Numero do endereço do consumidor é obrigatório.")
            .NotNull().WithMessage("Numero do endereço do consumidor é obrigatório.");
            RuleFor(s => s.Address.District)
            .NotEmpty().WithMessage("Bairro do consumidor é obrigatório.")
            .NotNull().WithMessage("Bairro do consumidor é obrigatório.");
            RuleFor(s => s.Address.Zip_code)
            .NotEmpty().WithMessage("CEP do consumidor é obrigatório.")
            .NotNull().WithMessage("CEP do consumidor é obrigatório.");
            RuleFor(s => s.Address.Latitude)
            .NotEmpty().WithMessage("Latitude do consumidor é obrigatório.")
            .NotNull().WithMessage("Latitude do consumidor é obrigatório.");
            RuleFor(s => s.Address.Longitude)
            .NotEmpty().WithMessage("Longitude do consumidor é obrigatório.")
            .NotNull().WithMessage("Longitude do consumidor é obrigatório.");
            RuleFor(s => s.Payments)
             .NotEmpty().WithMessage("Forma de pagamento é obrigatório.")
             .NotNull().WithMessage("Forma de pagamento é obrigatório.")
             .Must(s => s.Count > 0).WithMessage("Forma de pagamento é obrigatório.");
            RuleFor(s => s.Created_by)
            .NotEmpty().WithMessage("Created_by é obrigatório.")
            .NotNull().WithMessage("Created_by é obrigatório.");

        }
    }
}
