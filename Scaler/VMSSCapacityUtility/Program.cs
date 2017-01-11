using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Resource.Fluent;
using Microsoft.Azure.Management.Resource.Fluent.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSSCapacityUtility
{
    class Program
    {
        static string AzureSubscriptionId = "";
        static string AzureClientId = "";
        static string AzureClientKey = "";
        static string AzureTenantId = "";
        static string ResourceGroup = "ScaleSetRepro";
        static string ScaleSetName = "ScaleSetSample";
        
        static string ScaleSetId => $"/subscriptions/{AzureSubscriptionId}/resourceGroups/{ResourceGroup}/providers/Microsoft.Compute/virtualMachineScaleSets/{ScaleSetName}";

        static void Main(string[] args)
        {
            Console.WriteLine("VMMS Scaling Utility");
            Console.WriteLine("--------------------");
            Console.WriteLine();

            // Log into Azure
            Console.Write($"Logging into Azure subscription {AzureSubscriptionId}");
            var credentials = AzureCredentials.FromServicePrincipal(AzureClientId, AzureClientKey, AzureTenantId, AzureEnvironment.AzureGlobalCloud);
            var AzureClient = Azure.Authenticate(credentials).WithSubscription(AzureSubscriptionId);
            if (AzureClient?.SubscriptionId == AzureSubscriptionId)
            {
                Console.WriteLine("Successfully logged into Azure");
            }
            else
            {
                Console.WriteLine("ERROR: Failed to login to Azure");
            }

            Console.WriteLine($"Retrieving scale set {ResourceGroup}\\{ScaleSetName}");
            var scaleSet = AzureClient?.VirtualMachineScaleSets.GetById(ScaleSetId);
            var oldCapacity = scaleSet.Capacity;

            Console.WriteLine($"Current capacity: {oldCapacity}");
            Console.WriteLine($"Changing capacity to {oldCapacity + 1}...");
            scaleSet.Update().WithCapacity(oldCapacity + 1).Apply();

            Console.Write("- Done -");
        }
    }
}
