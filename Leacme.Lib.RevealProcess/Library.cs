// Copyright (c) 2017 Leacme (http://leac.me). View LICENSE.md for more information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Humanizer.Bytes;

namespace Leacme.Lib.RevealProcess {

	public class Library {

		public Library() {

		}

		/// <summary>
		/// Get list of processes for the local machine.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<PrunedProcess> GetCurrentProcesses() {
			return Process.GetProcesses().Select(z => new PrunedProcess(z));
		}

		/// <summary>
		/// Immediately try to force stop a process by its ID.
		/// /// </summary>
		/// <param name="processId"></param>
		public void KillProcessById(int processId) {
			Process.GetProcessById(processId).Kill();
		}

		/// <summary>
		/// Performs a calculation on the process CPU utilization in percent.
		/// /// </summary>
		/// <param name="processId"></param>
		/// <returns></returns>
		public static async Task<double> GetCpuUsageForProcessById(int processId) {
			var tempTime = DateTime.UtcNow;
			var tempTPT = Process.GetProcessById(processId).TotalProcessorTime;
			await Task.Delay(500);
			return Math.Round((Process.GetProcessById(processId).TotalProcessorTime - tempTPT).TotalMilliseconds / (Environment.ProcessorCount * (DateTime.UtcNow - tempTime).TotalMilliseconds) * 100);
		}

	}

	/// <summary>
	/// Streamlined process class.
	/// /// </summary>
	public class PrunedProcess {
		public string ProcessName { get; private set; }
		public int Id { get; private set; }
		public double MemoryKB { get; private set; }
		public double CPU { get; private set; } = 0;
		public int Handles { get; private set; }
		public bool Responding { get; private set; }

		public PrunedProcess(Process p) {
			Task.Run(async () => {
				await UpdateProcessAsync(p);
			});

		}

		public async Task UpdateProcessAsync(Process p) {
			ProcessName = p.ProcessName;
			Id = p.Id;
			MemoryKB = Math.Round(ByteSize.FromBytes(p.WorkingSet64).Kilobytes);
			Handles = p.HandleCount;
			Responding = p.Responding;
			CPU = await Library.GetCpuUsageForProcessById(p.Id);
		}
	}

}