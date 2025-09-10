using PropertyChanged;
using SUP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace SUP.ViewModels;
[AddINotifyPropertyChangedInterface]

public class GameViewModel
{
    

    IRandomService _randomService;

    Random random = new Random();

    List<Button> cards = new List<Button>();

public void GetRandomNumber()
    {
        
        List<int> randomNumbers = new List<int>();

        for (int i = 0; i < 10; i++)
        {
            randomNumbers.Add(i);
            randomNumbers.Add(i);

        }

        randomNumbers = randomNumbers.OrderBy(x => random.Next()).ToList();

        for(int i = 0; i < randomNumbers.Count; i++)
        {
            cards[i].Content = randomNumbers[i];
        }

        /*
         * Content källa: https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.contentcontrol.content?view=windowsdesktop-9.0#system-windows-controls-contentcontrol-content
         * Ska vi göra en binding med innehållet så att x:name eller liknande blir siffran? baserat på visibility
         * 
         * Basically, like MaxB said, every control in WPF has a "Visibility" property, that you can change between Visible, Collapsed or Hidden.

            Since you already have a Handle for the Button_Click event, all you need to do now is give a name to your TextBlock with the x:Name property like-so : <TextBlock x:Name="MyTextBlock"/>

            Then, in the code of your handler, you can choose which Visibility to apply to the TextBlock according to the state of your boolean. You can access the TextBlock properties by the name you gave it in the XAML file, like-so : this.MyTextBlock.Visibility = Visibility.Hidden, for example.
         */

    }

    

}

