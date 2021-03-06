﻿using KSPModAdmin.Core.Controller;

namespace KSPModAdmin.Core
{
    public delegate void AsyncTaskStartedHandler(object sender);
    public delegate void AsyncTaskDoneHandler(object sender);
    public delegate void StartingKSPHandler(object sender);
    public delegate void LanguageChangedHandler(object sender);
    public delegate void KSPPathChangingHandler(string oldKSPPath, string newKSPPath);
    public delegate void KSPPathChangedHandler(string kspPath);

    /// <summary>
    /// The EventDitributor is the event central of KSP MA.
    /// It contains all relevant events that should be distributed app wide.
    /// Like KSPRootChanged or start/finish of critical tasks.
    /// Hook your class to the events your plugin needs.
    /// </summary>
    public static class EventDistributor
    {
        /// <summary>
        /// Event for selected KSP paths changing.
        /// Occurs when the selected KSP paths is changing (before the change).
        /// </summary>
        public static event KSPPathChangingHandler KSPRootChanging = null;

        /// <summary>
        /// Event for selected KSP paths changed.
        /// Occurs when the selected KSP paths has changed.
        /// </summary>
        public static event KSPPathChangedHandler KSPRootChanged = null;

        /// <summary>
        /// Event for a controller has started a AsyncTask that needs to disables all other controller
        /// controls to prevent the user to perform two critical tasks at the same time.
        /// </summary>
        /// <remarks>Automatically hooked by derived class of BaseController.</remarks>
        public static event AsyncTaskStartedHandler AsyncTaskStarted = null;

        /// <summary>
        /// Event for a controller has finished its AsyncTask controls could be enabled again.
        /// </summary>
        /// <remarks>Automatically hooked by derived class of BaseController.</remarks>
        public static event AsyncTaskDoneHandler AsyncTaskDone = null;

        /// <summary>
        /// Event for starting KSP.
        /// Occurs when the KSP gets started (before the start).
        /// </summary>
        public static event StartingKSPHandler StartingKSP = null;

        /// <summary>
        /// Event for LanguageChanged.
        /// Occurs when the language got changed.
        /// </summary>
        public static event LanguageChangedHandler LanguageChanged = null;


        /// <summary>
        /// Static constructor.
        /// </summary>
        static EventDistributor()
        {
            OptionsController.KSPPathChanging += KSPPathChanging;
            OptionsController.KSPPathChanged += KSPPathChanged;
        }


        /// <summary>
        /// Invokes the AsyncTaskStarted event to inform other controller of the start of a critical task.
        /// All other controllers should disable their controls.
        /// </summary>
        /// <param name="sender"></param>
        public static void InvokeAsyncTaskStarted(object sender)
        {
            if (AsyncTaskStarted != null)
                AsyncTaskStarted(sender);
        }

        /// <summary>
        /// Invokes the AsyncTaskDone event to inform other controller that the critical task has finished.
        /// The controller could enable their controls again.
        /// </summary>
        /// <param name="sender"></param>
        public static void InvokeAsyncTaskDone(object sender)
        {
            if (AsyncTaskDone != null)
                AsyncTaskDone(sender);
        }

        /// <summary>
        /// Invokes the StartingKSP event to inform all listeners.
        /// </summary>
        public static void InvokeStartingKSP(object sender)
        {
            if (StartingKSP != null)
                StartingKSP(sender);
        }

        /// <summary>
        /// Invokes the LanguageChanged event to inform all listeners.
        /// </summary>
        public static void InvokeLanguageChanged(object sender)
        {
            if (LanguageChanged != null)
                LanguageChanged(sender);
        }


        /// <summary>
        /// Event handler for the KSPPathChanging event of the OptionsController.
        /// It invokes the KSPRootChanging event to inform all listeners that the KSP path will change.
        /// </summary>
        /// <param name="oldKSPPath">The old KSP path.</param>
        /// <param name="newKSPPath">The new KSP path.</param>
        private static void KSPPathChanging(string oldKSPPath, string newKSPPath)
        {
            if (KSPRootChanging != null)
                KSPRootChanging(oldKSPPath, newKSPPath);
        }

        /// <summary>
        /// Event handler for the KSPPathChanged event of the OptionsController.
        /// It invokes the KSPRootChanged event to inform all listeners that the KSP path has changed.
        /// </summary>
        /// <param name="kspPath">The new KSP path.</param>
        private static void KSPPathChanged(string kspPath)
        {
            if (KSPRootChanged != null)
                KSPRootChanged(kspPath);
        }
    }
}
