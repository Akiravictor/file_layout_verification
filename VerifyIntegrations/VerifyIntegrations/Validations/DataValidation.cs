using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using VerifyIntegrations.Models;
using VerifyIntegrations.Utils;

namespace VerifyIntegrations.Validations
{
	public class DataValidation
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(DataValidation));

		public void DataValidationMenu()
		{
			log.Info("Execution Start");

			string op = "";

			Console.Clear();

			while (!op.Equals("4"))
			{
				Console.WriteLine(" ==============================");
				Console.WriteLine("\t MENU DADOS");
				Console.WriteLine(" ==============================");

				Console.WriteLine("  1- Verificar Pasta ({0})", ConfigurationManager.AppSettings["InputFolder"].ToString());
				Console.WriteLine("  2- Validar UM arquivo AUTOMATICAMENTE");
				Console.WriteLine("  3- Validar UM arquivo MANUALMENTE");
				Console.WriteLine("  4- Voltar");

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
					log.Info("Calling ValidateFile");
					ValidateFile();
				}
				else if (op.Equals("3"))
				{
					log.Info("Calling ManualValidateFile");
					ManualValidateFile();
				}
				else if (op.Equals("4"))
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

		private void ValidateFile()
		{
			string op = "";
			Dictionary<int, string> files;
			Root layout;

			Console.Clear();

			do
			{
				log.Info("Calling ListFiles");
				Console.WriteLine(" Seleciona um arquivo");
				files = Tools.ListFiles(ConfigurationManager.AppSettings["InputFolder"].ToString());

				if (files.Count == 0)
				{
					Console.Clear();
					return;
				}

				Console.Write(" Opção: ");
				op = Console.ReadLine();

				if (!int.TryParse(op, out int result))
				{
					op = "0";
					Console.WriteLine(" Opção Inválida...");
					Thread.Sleep(1000);
					Console.Clear();
				}
				else if (int.Parse(op) < files.Keys.First() || int.Parse(op) > files.Keys.Last())
				{
					op = "0";
					Console.WriteLine(" Opção Inválida...");
					Thread.Sleep(1000);
					Console.Clear();
				}
				else
				{
					string[] file = Path.GetFileNameWithoutExtension(files[int.Parse(op)]).Split('_');

					log.Info("Calling LoadSingleLayout");
					layout = Tools.LoadSingleLayout(file[1], file[3]);

					if (layout == null)
					{
						Console.Clear();
						return;
					}
					else
					{
						log.Info("Calling DataValidationSubMenu");
						DataValidationSubMenu(layout, files[int.Parse(op)]);
					}

				}
			}
			while (int.Parse(op) < files.Keys.First() || int.Parse(op) > files.Keys.Last());

		}

		private void ManualValidateFile()
		{
			string op;
			Dictionary<int, string> files;
			Root layout;

			do
			{
				log.Info("Calling ListFiles");
				Console.WriteLine(" Seleciona um Layout");
				files = Tools.ListFiles(ConfigurationManager.AppSettings["LayoutFolder"].ToString());

				if (files.Count == 0)
				{
					Console.Clear();
					return;
				}

				Console.Write(" Opção: ");
				op = Console.ReadLine();

				if (!int.TryParse(op, out int result))
				{
					op = "0";
					Console.WriteLine(" Opção Inválida...");
					Thread.Sleep(1000);
					Console.Clear();
				}
				else if (int.Parse(op) < files.Keys.First() || int.Parse(op) > files.Keys.Last())
				{
					op = "0";
					Console.WriteLine(" Opção Inválida...");
					Thread.Sleep(1000);
					Console.Clear();
				}
				else
				{
					string[] file = Path.GetFileNameWithoutExtension(files[int.Parse(op)]).Split('_');

					log.Info("Calling LoadSingleLayout");
					layout = Tools.LoadSingleLayout(file[0], file[1]);

					if (layout == null)
					{
						Console.Clear();
						return;
					}
					else
					{
						do
						{
							log.Info("Calling ListFiles");
							Console.WriteLine(" Seleciona um Layout");
							files = Tools.ListFiles(ConfigurationManager.AppSettings["InputFolder"].ToString());

							if (files.Count == 0)
							{
								Console.Clear();
								return;
							}

							Console.Write(" Opção: ");
							op = Console.ReadLine();

							if (!int.TryParse(op, out int result2))
							{
								op = "0";
								Console.WriteLine(" Opção Inválida...");
								Thread.Sleep(1000);
								Console.Clear();
							}
							else if (int.Parse(op) < files.Keys.First() || int.Parse(op) > files.Keys.Last())
							{
								op = "0";
								Console.WriteLine(" Opção Inválida...");
								Thread.Sleep(1000);
								Console.Clear();
							}
							else
							{
								log.Info("Calling DataValidationSubMenu");
								DataValidationSubMenu(layout, files[int.Parse(op)]);
							}
						}
						while (int.Parse(op) < files.Keys.First() || int.Parse(op) > files.Keys.Last());
					}
				}
			}
			while (int.Parse(op) < files.Keys.First() || int.Parse(op) > files.Keys.Last());

			log.Info("Returning");
		}

		private void DataValidationSubMenu(Root layout, string file)
		{
			string op = "";

			while(!op.Equals("1") && !op.Equals("2"))
			{
				Console.Clear();
				Console.WriteLine(" Arquivo selecionado: {0}", Path.GetFileName(file));
				Console.WriteLine(" Layout: {0} Versão: {1}\n", layout.Layout.Name, layout.Layout.Version);
				Console.WriteLine(" 1- Verificar preenchimento de campos Obrigatórios");
				Console.WriteLine(" 2- Verificar preenchimento Completo");
				Console.Write("\n Opção: ");
				op = Console.ReadLine();

				if (!op.Equals("1") && !op.Equals("2"))
				{
					Console.WriteLine(" Opção Inválida...");
					Thread.Sleep(1000);
				}
			}

			Console.Clear();
			Console.WriteLine(" Arquivo selecionado: {0}", Path.GetFileName(file));
			Console.WriteLine(" Layout: {0} Versão: {1}\n", layout.Layout.Name, layout.Layout.Version);

			if (op.Equals("1"))
			{
				log.Info("Calling VerifyRequired");
				int errors = VerifyRequired(layout, file, out var LineErrors);

				Console.WriteLine(" Erros encontrados: {0}", errors);

				foreach (var erro in LineErrors)
				{
					Console.WriteLine(" Linha: {0}\n    {1}", erro.Key, erro.Value);
				}

				Tools.Pause();

			}
			else
			{
				log.Info("Calling FullVerify");
				int errors = FullVerify(layout, file, out Dictionary<int, string> LineErrors, out Dictionary<int, string> LineWarnings);

				Console.WriteLine(" Erros encontrados: {0}", errors);
				Console.WriteLine(" Avisos encontrados: {0}", LineWarnings.Count);

				if (LineErrors.Count > 0 && LineWarnings.Count == 0)
				{
					log.Info("Calling GenerateErrorLog");
					Thread thread = new Thread(
						new ThreadStart(() =>
							Tools.GenerateErrorLog(LineErrors.ToDictionary(x => x.Key, x => x.Value), null, Path.GetFileNameWithoutExtension(file))
							)
						);
					thread.Start();
				}
				else if (LineErrors.Count == 0 && LineWarnings.Count > 0)
				{
					log.Info("Calling GenerateErrorLog");
					Thread thread = new Thread(
						new ThreadStart(() =>
							Tools.GenerateErrorLog(null, LineWarnings.ToDictionary(x => x.Key, x => x.Value), Path.GetFileNameWithoutExtension(file))
							)
						);
					thread.Start();
				}
				else if (LineErrors.Count > 0 && LineWarnings.Count > 0)
				{
					log.Info("Calling GenerateErrorLog");
					Thread thread = new Thread(
						new ThreadStart(() =>
							Tools.GenerateErrorLog(LineErrors.ToDictionary(x => x.Key, x => x.Value), LineWarnings.ToDictionary(x => x.Key, x => x.Value), Path.GetFileNameWithoutExtension(file))
							)
						);
					thread.Start();
				}

				Thread.Sleep(1000);

				if (LineWarnings.Count > 0)
				{
					Console.WriteLine("\n Lista de Avisos:");

					string page = "1";
					int lines = 0;

					while (page.Equals("1") && lines < LineWarnings.Count)
					{
						page = "";

						var print = LineWarnings.Take(int.Parse(ConfigurationManager.AppSettings["ItemsPerPage"].ToString())).ToDictionary(l => l.Key, l => l.Value);

						foreach (var warning in print)
						{
							Console.WriteLine(" Linha: {0}\n    {1}", warning.Key, warning.Value);
							lines++;
							LineWarnings.Remove(warning.Key);
						}

						Console.WriteLine("\n Continuar?\n 1- Sim  2- Não");
						Console.Write(" Opção: ");
						page = Console.ReadLine();

						Console.Clear();
					}

				}
				
				if (LineErrors.Count > 0)
				{
					Console.WriteLine("\n Lista de Erros:");

					string page = "1";
					int lines = 0;

					while (page.Equals("1") && lines < LineErrors.Count)
					{
						page = "";

						var print = LineErrors.Take(int.Parse(ConfigurationManager.AppSettings["ItemsPerPage"].ToString())).ToDictionary(l => l.Key, l => l.Value);
						
						foreach (var erro in print)
						{
							Console.WriteLine(" Linha: {0}\n    {1}", erro.Key, erro.Value);
							lines++;

							LineErrors.Remove(erro.Key);
						}

						Console.WriteLine("\n Continuar?\n 1- Sim  2- Não");
						Console.Write(" Opção: ");
						page = Console.ReadLine();

						Console.Clear();
					}				
				}

				if (LineErrors.Count == 0 && LineWarnings.Count == 0)
				{
					Console.WriteLine(" Deseja mover o arquivo {0} para a pasta de arquivos válidos?\n 1- Sim  2- Não", Path.GetFileName(file));
					Console.Write(" Opção: ");
					op = Console.ReadLine();

					if (op.Equals("1"))
					{
						log.Info("Calling MoveToValid");
						Tools.MoveToValid(file);
					}
				}
				else
				{
					Console.WriteLine(" Deseja mover o arquivo {0} para a pasta de arquivos inválidos?\n 1- Sim  2- Não", Path.GetFileName(file));
					Console.Write(" Opção: ");
					op = Console.ReadLine();

					if (op.Equals("1"))
					{
						log.Info("Calling MoveToInvalid");
						Tools.MoveToInvalid(file);
					}
				}
			}

			Console.Clear();
			log.Info("Returning");

		}

		private int VerifyRequired(Root layout, string file, out Dictionary<int,string> LineError)
		{
			List<string> RequiredFields = layout.Layout.Fields.Where(l => l.isRequired == 1).Select(l => l.Name).ToList();
			LineError = new Dictionary<int, string>();

			using (StreamReader sr = new StreamReader(file))
			{
				string header = sr.ReadLine();
				string line = string.Empty;
				Dictionary<int, string> RequiredFieldsIndex = new Dictionary<int, string>();

				int lineNum = 2;

				for (int i = 0; i < header.Split('\t').Length; i++)
				{
					foreach (var reqField in RequiredFields)
					{
						if (reqField.Equals(header.Split('\t')[i]))
						{
							RequiredFieldsIndex.Add(i, reqField);
						}
					}
				}

				log.Info("Verifying Required Fields");
				if (RequiredFieldsIndex.Keys.Count < RequiredFields.Count)
				{
					foreach (var item in RequiredFields.Where(r => !RequiredFieldsIndex.ContainsValue(r)).ToList())
					{
						Console.WriteLine(" {0} é obrigatório e não foi encontrado...", item);
					}

					return RequiredFields.Where(r => !RequiredFieldsIndex.ContainsValue(r)).ToList().Count;
				}
				else
				{
					log.Info("Verifying file content");

					while ((line = sr.ReadLine()) != null)
					{
						foreach (int idx in RequiredFieldsIndex.Keys)
						{
							if (line.Split('\t')[idx].Trim().Equals(""))
							{
								LineError.Add(lineNum, string.Format("Campo obrigatório faltando: {0}.", RequiredFieldsIndex[idx]));
							}
						}

						lineNum++;
					}
				}

				sr.Close();
			}

			log.Info("Returning");
			return LineError.Count;
		}

		private int FullVerify(Root layout, string file, out Dictionary<int, string> LineError, out Dictionary<int, string> LineWarning)
		{
			List<string> RequiredFields = layout.Layout.Fields.Where(l => l.isRequired == 1).Select(l => l.Name).ToList();
			List<string> PrimaryKeys = layout.Layout.Fields.Where(l => l.isPrimaryKey == 1).Select(l => l.Name).ToList();
			List<Field> AllFields = layout.Layout.Fields;
			LineError = new Dictionary<int, string>();
			LineWarning = new Dictionary<int, string>();

			int lineNum = 1;

			using (StreamReader sr = new StreamReader(file))
			{
				Dictionary<int, string> AvailableColumns = new Dictionary<int, string>();
				Dictionary<int, string> RequiredFieldsIndex = new Dictionary<int, string>();
				Dictionary<int, string> PrimaryKeysIndex = new Dictionary<int, string>();

				string header = sr.ReadLine();
				string line = string.Empty;

				for (int i = 0; i < header.Split('\t').Length; i++)
				{
					foreach(var field in AllFields)
					{
						if (field.Name.Equals(header.Split('\t')[i]))
						{
							AvailableColumns.Add(i, header.Split('\t')[i]);

							if (field.isRequired == 1)
							{
								RequiredFieldsIndex.Add(i, header.Split('\t')[i]);
							}
							if (field.isPrimaryKey == 1)
							{
								PrimaryKeysIndex.Add(i, header.Split('\t')[i]);
							}
						}
					}
				}

				List<string> list;

				log.Info("Verifying fields out of layout");
				if ( (list = header.Split('\t').ToList().Where(h => !AllFields.Select(af => af.Name).Contains(h)).ToList()).Count > 0 )
				{
					foreach(var item in list)
					{
						if (LineWarning.ContainsKey(lineNum))
						{
							LineWarning[lineNum] = string.Format(" {0}\n    O campo {1} não pertence ao layout {2} de versão {3} e será ignorado...", LineWarning[lineNum], item, layout.Layout.Name, layout.Layout.Number);
						}
						else
						{
							LineWarning.Add(lineNum, string.Format("    O campo {0} não pertence ao layout {1} de versão {2} e será ignorado...", item, layout.Layout.Name, layout.Layout.Number));
						}
					}
				}

				log.Info("Verifying Required Fields");
				if (RequiredFields.Count > RequiredFieldsIndex.Keys.Count)
				{
					list = RequiredFields.Where(r => !RequiredFieldsIndex.ContainsValue(r)).ToList();

					foreach (var item in list)
					{
						if (LineError.ContainsKey(lineNum))
						{
							LineError[lineNum] = string.Format(" {0}\n    O campo {1} é obrigatório e não foi encontrado...", LineError[lineNum], item);
						}
						else
						{
							LineError.Add(lineNum, string.Format("O campo {0} é obrigatório e não foi encontrado...", item));
						}
					}

					return list.Count;
				}

				log.Info("Verifying Primary Keys");
				if (PrimaryKeys.Count > PrimaryKeysIndex.Keys.Count)
				{
					list = PrimaryKeys.Where(p => !PrimaryKeysIndex.ContainsValue(p)).ToList();

					foreach (var item in list)
					{
						if (LineError.ContainsKey(lineNum))
						{
							LineError[lineNum] = string.Format(" {0}\n    O campo {1} é chave primária e não foi encontrado...", LineError[lineNum], item);
						}
						else
						{
							LineError.Add(lineNum, string.Format("O campo {0} é chave primária e não foi encontrado...", item));
						}
					}

					return list.Count;
				}
				else
				{
					log.Info("Verifying file content");

					while ((line = sr.ReadLine()) != null)
					{
						foreach (int idx in AvailableColumns.Keys)
						{
							string lineField = line.Split('\t')[idx].Trim();
							Field currentField = AllFields.First(f => f.Name == AvailableColumns[idx]);

							if (currentField.isRequired == 1 && lineField.Equals(""))
							{
								if (LineError.ContainsKey(lineNum))
								{
									LineError[lineNum] = string.Format(" {0}\n    O campo {1} é obrigatório.", LineError[lineNum], AvailableColumns[idx]);
								}
								else
								{
									LineError.Add(lineNum, string.Format("O campo {0} é obrigatório.", AvailableColumns[idx]));
								}
							}

							if (currentField.isPrimaryKey == 1 && lineField.Equals(""))
							{
								if (LineError.ContainsKey(lineNum))
								{
									LineError[lineNum] = string.Format(" {0}\n    O campo {1} é chave-primária.", LineError[lineNum], AvailableColumns[idx]);
								}
								else
								{
									LineError.Add(lineNum, string.Format("O campo {0} é chave-primária.", AvailableColumns[idx]));
								}
							}

							if (currentField.DataType.Equals("INTEGER") && currentField.isRequired == 1 && !int.TryParse(lineField, out int intRes) )
							{
								if (LineError.ContainsKey(lineNum))
								{
									LineError[lineNum] = string.Format("{0}\n    O campo {1} é tipo INTEGER, mas {2} não é do tipo INTEGER.", LineError[lineNum], AvailableColumns[idx], lineField);
								}
								else
								{
									LineError.Add(lineNum, string.Format("O campo {0} é tipo INTEGER, mas {1} não é do tipo INTEGER.", AvailableColumns[idx], lineField));
								}
							}

							if (currentField.DataType.Equals("DOUBLE") && currentField.isRequired == 1 && !double.TryParse(lineField, out double dbRes))
							{
								if (LineError.ContainsKey(lineNum))
								{
									LineError[lineNum] = string.Format("{0}\n     O campo {1} é tipo DOUBLE, mas {2} não é do tipo DOUBLE.", LineError[lineNum], AvailableColumns[idx], lineField);
								}
								else
								{
									LineError.Add(lineNum, string.Format("O campo {0} é tipo DOUBLE, mas {1} não é do tipo DOUBLE.", AvailableColumns[idx], lineField));
								}
							}

							if (currentField.DataType.Equals("DATE") && currentField.isRequired == 1 && !DateTime.TryParse(lineField, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime dtRes))
							{
								if (LineError.ContainsKey(lineNum))
								{
									LineError[lineNum] = string.Format("{0}\n    O campo {1} é tipo DATE, mas {2} não é do tipo DATE.", LineError[lineNum], AvailableColumns[idx], lineField);
								}
								else
								{
									LineError.Add(lineNum, string.Format("O campo {0} é tipo DATE, mas {1} não é do tipo DATE.", AvailableColumns[idx], lineField));
								}
							}

							if (currentField.RegexValidation != null)
							{
								string regex = currentField.RegexValidation;

								Match match = Regex.Match(lineField, regex, RegexOptions.IgnoreCase);

								if (!match.Success && !lineField.Equals("") && currentField.isRequired == 1)
								{
									if (LineError.ContainsKey(lineNum))
									{
										LineError[lineNum] = string.Format("{0}\n    O campo {1} não possui o padrão especificado.", LineError[lineNum], AvailableColumns[idx]);
									}
									else
									{
										LineError.Add(lineNum, string.Format("O campo {0} não possui o padrão especificado.", AvailableColumns[idx]));
									}
								}
							}

							if (currentField.LengthValidation != null)
							{
								var length = currentField.LengthValidation;

								if (!length.Contains(lineField.Length) && currentField.isRequired == 1)
								{
									if (LineError.ContainsKey(lineNum))
									{
										LineError[lineNum] = string.Format("{0}\n    O campo {1} não possui a quantia de caracteres especificada.", LineError[lineNum], AvailableColumns[idx]);
									}
									else
									{
										LineError.Add(lineNum, string.Format("O campo {0} não possui a quantia de caracteres especificada.", AvailableColumns[idx]));
									}
								}
							}

							if (currentField.Conditional != null)
							{
								var conditional = currentField.Conditional;



							}

							if (currentField.MechanicRequired != null)
							{
								var mechanic = currentField.MechanicRequired;

								if (AvailableColumns.ContainsValue("MECHANIC"))
								{
									string mechFound = line.Split('\t')[AvailableColumns.FirstOrDefault(a => a.Value.Equals("MECHANIC")).Key].Trim();

									if (mechanic.Contains(mechFound))
									{
										if (lineField.Equals(""))
										{
											if (LineError.ContainsKey(lineNum))
											{
												LineError[lineNum] = string.Format("{0}\n    O campo {1} deve ser preenchido por causa da mecânica {2}.", LineError[lineNum], AvailableColumns[idx], mechFound);
											}
											else
											{
												LineError.Add(lineNum, string.Format("O campo {0} deve ser preenchido por causa da mecânica {1}.", AvailableColumns[idx], mechFound));
											}
										}
									}
								}
							}

							if (currentField.ValidValues != null)
							{
								var valid = currentField.ValidValues;

								if (!valid.Contains(lineField) && currentField.isRequired == 1)
								{
									if (LineError.ContainsKey(lineNum))
									{
										LineError[lineNum] = string.Format("{0}\n    O valor do campo {1} não está na lista de valores aceitos.", LineError[lineNum], AvailableColumns[idx]);
									}
									else
									{
										LineError.Add(lineNum, string.Format("O valor do campo {0} não está na lista de valores aceitos..", AvailableColumns[idx]));
									}
								}
							}
						}

						lineNum++;
					}
				}
			}

			log.Info("Returning");
			return LineError.Count;
		}
	}
}
