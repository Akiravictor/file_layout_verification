using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using VerifyIntegrations.Models;

namespace VerifyIntegrations.Utils
{
	public static class Tools
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Tools));
		public static Dictionary<int, string> ListFiles(string path)
		{
			Dictionary<int, string> FileList = new Dictionary<int, string>();

			if (Directory.Exists(path))
			{
				string[] files = Directory.GetFiles(path);

				Console.Clear();
				Console.WriteLine(" Arquivos no diretório:\n");

				if (files.Length > 0)
				{
					int i = 1;

					foreach (var file in files)
					{
						Console.WriteLine(" {0}- {1}", i, Path.GetFileName(file.ToString()));
						FileList.Add(i, file);
						i++;
					}
				}
				else
				{
					Console.WriteLine(" O diretório está vazio...");
				}
			}
			else
			{
				Console.WriteLine(" O diretório {0} não foi encontrado...", path);
				Pause();
			}

			return FileList;
		}

		public static Dictionary<string, Root> LoadLayouts()
		{
			Dictionary<string, Root> Layouts = new Dictionary<string, Root>();

			if (Directory.Exists(ConfigurationManager.AppSettings["LayoutFolder"].ToString()))
			{
				string[] files = Directory.GetFiles(ConfigurationManager.AppSettings["LayoutFolder"].ToString());

				if (files.Length > 0)
				{
					foreach (var file in files)
					{
						using (StreamReader rs = new StreamReader(file))
						{
							try
							{
								Layouts.Add(Path.GetFileNameWithoutExtension(file), JsonConvert.DeserializeObject<Root>(rs.ReadToEnd().ToString()));
							}
							catch (JsonSerializationException e)
							{
								Console.WriteLine(" Ocorreu um erro ao Deserializar o JSON {0}:\n {1}", Path.GetFileName(file), e.Message.ToString());
								Pause();

							}
							
						}
					}
				}
				else
				{
					Console.WriteLine(" Nenhum arquivo de layout encontrado no diretório {0}...", ConfigurationManager.AppSettings["LayoutFolder"].ToString());
					Pause();
				}
			}
			else
			{
				Console.WriteLine(" O diretório de layouts {0} não foi encontrado...", ConfigurationManager.AppSettings["LayoutFolder"].ToString());
				Pause();
			}

			return Layouts;
		}

		public static Root LoadSingleLayout(string LayoutName, string Version)
		{
			Root layout = null;

			if (Directory.Exists(ConfigurationManager.AppSettings["LayoutFolder"].ToString()))
			{
				string files = Directory.GetFiles(ConfigurationManager.AppSettings["LayoutFolder"].ToString()).FirstOrDefault(f => f.Contains(string.Format("{0}_{1}", LayoutName, Version)));

				using (StreamReader rs = new StreamReader(files))
				{
					try
					{
						layout = JsonConvert.DeserializeObject<Root>(rs.ReadToEnd().ToString());
					}
					catch (JsonSerializationException e)
					{
						Console.WriteLine(" Ocorreu um erro ao Deserializar o JSON {0}:\n {1}", Path.GetFileName(files), e.Message.ToString());
						Pause();
					}

				}
			}
			else
			{
				Console.WriteLine(" O diretório {0} não foi encontrado...", Directory.Exists(ConfigurationManager.AppSettings["LayoutFolder"].ToString()));
				Pause();
			}

			return layout;
		}

		public static void MoveToInvalid(string file)
		{
			if (Directory.Exists(ConfigurationManager.AppSettings["InvalidFolder"].ToString()))
			{
				Console.WriteLine(" Movendo {0} para pasta de Inválidos...\n", Path.GetFileName(file));
				File.Move(file, string.Format("{0}\\{1}", ConfigurationManager.AppSettings["InvalidFolder"].ToString(), Path.GetFileName(file)));
			}
			else
			{
				Console.WriteLine(" O diretório de arquivos inválidos ({0}) não foi encontrado...", ConfigurationManager.AppSettings["InvalidFolder"].ToString());
			}
		}

		public static void MoveToValid(string file)
		{
			if (Directory.Exists(ConfigurationManager.AppSettings["ValidFolder"].ToString()))
			{
				Console.WriteLine(" Movendo {0} para pasta de Válidos...\n", Path.GetFileName(file));
				File.Move(file, string.Format("{0}\\{1}", ConfigurationManager.AppSettings["ValidFolder"].ToString(), Path.GetFileName(file)));
			}
			else
			{
				Console.WriteLine(" O diretório de arquivos Válidos ({0}) não foi encontrado...", ConfigurationManager.AppSettings["ValidFolder"].ToString());
			}
		}

		public static void Pause()
		{
			Console.WriteLine("\n\n Pressione qualquer tecla para continuar...");
			Console.ReadLine();
		}

		public static void GenerateErrorLog(Dictionary<int, string> Errors, Dictionary<int, string> Warnings, string file)
		{
			if (Directory.Exists(ConfigurationManager.AppSettings["LogFolder"].ToString()))
			{
				string path = Path.Combine(ConfigurationManager.AppSettings["LogFolder"].ToString(), string.Format("{0}_{1}.txt", file, DateTime.Now.ToString("yyyyMMdd_HHmm")));

				using (StreamWriter sw = File.CreateText(path))
				{
					if (Warnings != null)
					{
						sw.WriteLine("Avisos:\n");
						foreach (var warning in Warnings)
						{
							sw.WriteLine(" Linha: {0}\n    {1}", warning.Key, warning.Value);
						}
					}
					if (Errors != null)
					{
						sw.WriteLine("Erros:\n");
						foreach (var error in Errors)
						{
							sw.WriteLine(" Linha: {0}\n    {1}", error.Key, error.Value);
						}
					}
				}
			}
		}
	}
}
