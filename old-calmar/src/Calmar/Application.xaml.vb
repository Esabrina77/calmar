Class Application

    ' Les événements de niveau application, par exemple Startup, Exit et DispatcherUnhandledException
    ' peuvent être gérés dans ce fichier.

    Private Sub Application_Startup(sender As Object, e As System.Windows.StartupEventArgs) Handles Me.Startup

        ApplicationArgs = e.Args

        Version = My.Application.Info.Version.ToString
        Try
            Version = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString
            ApplicationArgs = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData
        Catch
        End Try

    End Sub

End Class
