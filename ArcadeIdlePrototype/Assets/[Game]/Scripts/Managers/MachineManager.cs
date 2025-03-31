using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZLinq;

public class MachineManager : MonoBehaviour
{
    [SerializeField] private List<Machine> Machines;

    public ItemStorage GetBy((MachineType machineType, bool isRawStorage) task, int minItemCount, Character characterCheckExclude,
                             bool onlyNonInteracting = false)
    {
        //zero allocation with ZLinq
        var machine = Machines.AsValueEnumerable()
                              .Where(m => (!onlyNonInteracting ||
                                           !m.ProductStorage.IsInteractingWithAgent
                                          ) //||
                                          //characterCheckExclude.InteractingStorage == m.ProductStorage)
                                          && m.MachineType == task.machineType
                                          && (m.ProductStorage.ItemCount >= minItemCount))
                              .OrderByDescending(m => task.isRawStorage ? m?.RawMaterialStorage.TotalSpace : m.ProductStorage.ItemCount)
                              .ThenBy(m => Vector3.Distance(m.ProductStorage.transform.position, characterCheckExclude.transform.position))
                              .FirstOrDefault();

        if (!machine)
            return null;
        return task.isRawStorage ? machine.RawMaterialStorage : machine.ProductStorage;
    }
}