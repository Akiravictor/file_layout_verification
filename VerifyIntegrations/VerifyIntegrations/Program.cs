using System;
using log4net;
using log4net.Config;
using System.Threading;
using VerifyIntegrations.Validations;

namespace VerifyIntegrations
{
	public class Program
	{

		private static readonly ILog log = LogManager.GetLogger(typeof(Program));
		static void Main(string[] args)
		{
			XmlConfigurator.Configure();

			string op = "";

			FileValidation fv = new FileValidation();
			DataValidation dv = new DataValidation();

			log.Info("Execution Start");

			while (true)
			{
				Console.WriteLine(" ==============================");
				Console.WriteLine("\t MENU PRINCIPAL");
				Console.WriteLine(" ==============================");

				Console.WriteLine("  1- Verificar Arquivos");
				Console.WriteLine("  2- Verificar Dados");
				Console.WriteLine("  3- Sair");

				Console.Write("\n  Opção: ");
				op = Console.ReadLine();

				if (op.Equals("1"))
				{
					fv.FileValidationMenu();
				}
				else if (op.Equals("2"))
				{
					dv.DataValidationMenu();
				}
				else if (op.Equals("3"))
				{
					Environment.Exit(0);
				}
				else
				{
					Console.WriteLine("Opção Inválida");
					Thread.Sleep(1000);
					Console.Clear();
				}
			}
		}
	}
}
