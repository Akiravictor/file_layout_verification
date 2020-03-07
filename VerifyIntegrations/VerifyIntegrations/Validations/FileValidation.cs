using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Threading;
using VerifyIntegrations.Models;
using VerifyIntegrations.Utils;

namespace VerifyIntegrations.Validations
{
	public class FileValidation
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(FileValidation));
		public void FileValidationMenu()
		{
			log.Info("Execution Start");

			string op = "";

			Console.Clear();

			while (!op.Equals("3"))
			{
				Console.WriteLine(" ==============================");
				Console.WriteLine("\t MENU ARQUIVO");
				Console.WriteLine(" ==============================");

				Console.WriteLine("  1- Verificar Pasta ({0})", ConfigurationManager.AppSettings["InputFolder"].ToString());
				Console.WriteLine("  2- Validar TODOS os arquivos");
				Console.WriteLine("  3- Voltar");

				Console.Write("\n  Opção: ");
				op = Console.ReadLine();

				if (op.Equals("1"))
				{
					log.Info("Calling ListFiles");
					Tools.ListFiles(ConfigurationManager.AppSettings["InputFolder"].ToString());
					Tools.Pause();
					Console.Clear();
				}
				else if (op.Equals("2"))
				{
					log.Info("Calling VerifyAllFiles");
					VerifyAllFiles();
				}
				else if (op.Equals("3"))
				{
					Console.Clear();
				}
				else
				{
					Console.WriteLine("Opção Inválida");
					Thread.Sleep(1000);
					Console.Clear();
				}
			}

			log.Info("Execution Ending");
		}

		private void VerifyAllFiles()
		{

			Console.Clear();

			log.Info("Calling LoadLayouts");
			Dictionary<string, Root> Layouts = Tools.LoadLayouts();

			if (Layouts.Count == 0)
			{
				Console.Clear();
				return;
			}

			List<string> InvalidFiles = new List<string>();

			log.Info("Checking InputFolder existance");
			if (Directory.Exists(ConfigurationManager.AppSettings["InputFolder"].ToString()))
			{
				string[] files = Directory.GetFiles(ConfigurationManager.AppSettings["InputFolder"].ToString());
				string op = "";

				log.Info("Performing validations");
				Console.WriteLine(" Verificando Arquivos no diretório:\n");

				if (files.Length > 0)
				{
					foreach (var file in files)
					{
						var fileName = Path.GetFileNameWithoutExtension(file.ToString());
						var split = fileName.Split('_');

						Console.Write(" {0} - ", fileName);

						if (split.Length >= 5)
						{
							bool fullMatch = false;

							foreach (var item in Layouts)
							{
								fullMatch = false;	

								if (split[0].Equals(item.Value.Layout.Domain.ToString())){
									fullMatch = true;
								}
								else
								{
									fullMatch = false;
								}

								if (split[1].Equals(item.Value.Layout.Name.ToString()) && fullMatch)
								{
									fullMatch &= true;
								}
								else
								{
									fullMatch = false;
								}

								if (split[2].Equals(item.Value.Layout.Number.ToString()) && fullMatch)
								{
									fullMatch &= true;
								}
								else
								{
									fullMatch = false;
								}

								if (split[3].Equals(item.Value.Layout.Version.ToString()) && fullMatch)
								{
									fullMatch &= true;
								}
								else
								{
									fullMatch = false;
								}

								if (fullMatch)
								{
									break;
								}
							}
							
							if (fullMatch && !DateTime.TryParseExact(split[4], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
							{
								Console.WriteLine("INVALIDO");
								InvalidFiles.Add(file);

							}
							else
							{
								Console.WriteLine("VALIDO");
							}
						}
						else
						{
							Console.WriteLine("INVALIDO");
							InvalidFiles.Add(file);
						}
					}
				}
				else
				{
					log.Error("InputFolder is empty");
					Console.WriteLine(" O diretório está vazio...");
				}
				
				while(!op.Equals("1") && !op.Equals("2"))
				{
					Console.WriteLine("\n Mover arquivos Inválidos para a pasta de Inválidos?\n 1- Sim  2- Não");
					Console.Write(" Opção: ");
					op = Console.ReadLine();

					if (op.Equals("1"))
					{
						foreach(var invalid in InvalidFiles)
						{
							log.Info("Calling MoveToInvalid");
							Tools.MoveToInvalid(invalid);
						}
					}
					else if (!op.Equals("2"))
					{
						Console.WriteLine("Opção inválida...");
					}
				}
				
				Console.WriteLine("\n\n Pressione qualquer tecla para continuar...");
				Console.ReadLine();
			}
			else
			{
				log.Error("InputFolder could not be found");
				Console.WriteLine(" O diretório {0} não foi encontrado...", ConfigurationManager.AppSettings["InputFolder"].ToString());
			}

			Console.Clear();
			log.Info("Returning");
		}
	}
}
