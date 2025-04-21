using System.Globalization;

namespace Questao1
{
    public class ContaBancaria {

        private double _saldoConta;
        public double SaldoConta => _saldoConta;

        public long NumeroConta {  get; init; }
        public string TitularConta { get; set; }

        //numero, titular, depositoInicial)
        public ContaBancaria(long numero,string titular, double depositoInicial = 0)
        {
            NumeroConta = numero;
            TitularConta = titular;
            _saldoConta += depositoInicial;
        }

        public void Deposito(double valorDeposito) => _saldoConta += valorDeposito;
        public void Saque(double valorSaque) => _saldoConta -= valorSaque + 3.50;

        public override string ToString()
        {
            return $"Conta {NumeroConta}, Titular: {TitularConta}, Saldo $ {SaldoConta:F2}";
        }
    }
}
