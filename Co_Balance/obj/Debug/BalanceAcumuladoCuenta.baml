   M S B A M L   `   `   `  ����E  ACo_Balance, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:Co_Balance
Co_Balance  5   	   '    !Co_Balance.BalanceAcumuladoCuenta   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:Co_Balance     5   	   a ]Syncfusion.SfGrid.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfGridConverter.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89_	 [Syncfusion.Grid.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j
 fSyncfusion.SfBusyIndicator.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.Shared.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c897SiaSf!http://schemas.syncfusion.com/wpf    	 
  5   	   -   5          � Title$  BalanceAcumuladoCuenta��5   	   $	��450��6(   $	��770��65   +  %Syncfusion.Windows.Shared.SkinStorage   VisualStyle$ Metro��6A    � WindowStartupLocation$ CenterScreen=�6~   .��5
      � .+�5   
   �  � 	Resources 5         botones-menu �� Property �� Value �� 
TargetType �� Triggers P� Property	 P� Value  Tit_dat
 �� 
TargetType 	 borde3  -System.Windows.Media.Effects.DropShadowEffect   	Direction   ShadowDepth   
BlurRadius   Color 	 title 	 value3  -Syncfusion.UI.Xaml.Grid.GridHeaderCellControl  headerStyle��%/  &	        &	 b    &	     &	 Q    &	 .    &	 5    �� 5      .�5      ��  � �� �#2196F36;   �� 5       � �� 6<   Z� �� 5       � ��
 �����6;   �� 5       � ��$ 40��67   �� 5       � ��$ Stretch=�6D   �� 5       � ��$ 0 0 5 0q�67   �� 5       � �� 5      �� 5      " ��6.   .R�5   "   �� #��z��6)   $��1q�6j   ��6S   Z� ._�5   &   �� $��Center=�67   $��Center=�6T   5   #   5      5       5      P� 5       �@J�IsMouseOver	 . . �5      ��  � �� �	#FF117A656C   �� 5        �@�Cursor$
 Hand��6?   5!      5#      �� 5$      .�5%      ��  � ��$ 10 0 0 0q�67   �� 5&       � �� �#03A9F46;   �� 5'       � ��$
 Bold"�6;   �� 5(       � ��$ 18(�69   5)      �� 5*      "
 ��6   .�5+      ��  � ��
 �����63   �� 5,       � ��$
 Auto��6/   �� 5-       � ��$ 1,1,1,1q�68   �� 5.       � ��$ 5q�6/   �� 5/       �@J�Effect 50        51      $ 0Q�6/   $	 0.5Q�6=   $ 5Q�6O   $ Black��6^   53      54      �� 55      .�56      ��  � ��$ 9(�69   �� 57       � ��$ Center=�6D   �� 58       � ��$ Center=�6B   �� 59       � �� �#2c3e506;   �� 5:       � ��$
 Bold"�6;   5;      �� 5<      .�5=      ��  � ��$ 11(�69   �� 5>       � ��$ Center=�6D   �� 5?       � ��$ Center=�6B   �� 5@       � �� �#2980b96;   �� 5A       � ��$
 Bold"�6;   �� 5B       � ��$ 5 0 5 0q�67   5C      �� 5D      "
  6   .�5E      ��  � ��$
 Bold"�63   �� 5F       � ��$ 12(�61   �� 5G       � ��
 �����63   �� 5H       � �� �#37373763   5I      5J      .+�5K      �  � RowDefinitions 5L      �� 5M      $��1* �6$   �� 5N      $��50 �6$    5P        	gridStyle&   Syncfusion.UI.Xaml.Grid.GridCell �� Binding �� Path �� Value��%   &	       &	 N    �� 5Q      "
  6/   .�5R      ��  � ��
 �����68    5S      �� 5T       6*   �� $ tip_cta� M6K   .a�5U   "   ��  � ��
 �����6?   �� 5V   "    � ��
 �  ��6G   �� 5W   "    � ��$ Courier New)�6G   �� 5X   "    � ��$ 11(�6E   6R   �� 5Y   "    � ��$
 Bold"�6G   6V   5Z      �� 5[       6*   �� $ tip_cta� A6K   .a�5\   "   ��  � �� �	#191E90FF6?   �� 5]   "    � ��
 ��  �6G   �� 5^   "    � ��$ Courier New)�6G   �� 5_   "    � ��$ 11(�6E   6R   5`      5b      �� 5c      "
  6   .�5d      ��  � ��$
 Bold"�67   �� 5e       � ��$ 11(�65   �� 5f       � ��
 �����67   �� 5g       � �� �#37373767   5h      5i      .+�5k      � -   5        $��GridSpan��5k   $   $��0��6   .+�5l      (  "Syncfusion.UI.Xaml.Grid.SfDataGrid  -   5          Name$ GridBalance��5l   n   (  "Syncfusion.UI.Xaml.Grid.SfGridBase   FrozenColumnCount$ 3��6'      AllowSorting .     AllowResizingColumns .    EnableDataVirtualization .    AutoGenerateColumns .     ShowGroupDropArea .  $��12(�6�   
 � Tag$ 1��6�      RowSelectionBrush �	#7F3498DB   HeaderStyle# [ 6     AllowFiltering .     	CellStyle# [ 6V     Columns 5n      0  *Syncfusion.UI.Xaml.Grid.GridTemplateColumn  ,  &Syncfusion.UI.Xaml.Grid.GridColumnBase    MappingName$  Detalle��67   !   Width$! 60Q�6M   (	  "Syncfusion.UI.Xaml.Grid.GridColumn" 	  AllowFiltering" .  #   CellTemplate# 5o   "   �� 5p   &   .R�5q   *   �� $ ��Name$$ 
BtnDetalle��61   -   5        $
��....��5q   E   ���#2196F3
�������6�   5r   '   5t      ,
  &Syncfusion.UI.Xaml.Grid.GridTextColumn
  5u      $	  ano��63   %   IsHidden% . 6E   
  5v      &   
HeaderText$& Periodo��63   $  per_doc��6H   $! 80Q�6^   
  5w      $& Saldo Inicial��63   $	! 150Q�6N   '   TextAlignment$' Right=�6�   (   DisplayBinding( 6Z   �� $ sal_ini�) �� StringFormat$) n2��6�   
  5x      $  cod_cta��63   % . 6I   
  5y      $& Debitos��63   $	! 150Q�6H   $' Right=�6�   ( 6T   �� $ debitos�$) n2��6�   
  5z      $& Creditos��63   $	! 150Q�6I   $' Right=�6�   ( 6U   �� $ creditos�$) n2��6�   
  5{      $& Saldo Final��63   $	! 150Q�6L   $' Right=�6�   ( 6X   �� $ sal_fin�$) n2��6�   5}      > 
 8Syncfusion.Windows.Controls.Notification.SfBusyIndicator  5~      -   5        $��sfBusyIndicator��5~   @   |�.  $��5q�6W   $��Bottom=�6b   $��12(�6}   *   AnimationSpeed$	* 100Q�6�   +   AnimationType$+ Windmill=�6�   ,   Header$!, Cargando Datos - SiasoftApp��6�   $	��200��6�   -   IsBusy- .  ���#2196F36�   5      � 5�      $��2��6   .+�5�        -   5        $ GridCosteoErrores��5�   '    .  .  $��10(�6x    . $ 1��6�    �	#7F3498DB# [ 6�   5�      � 5�      $��3��6   ���#373737.+�5�      �� . �� Orientation$. 
Horizontal=�6!   $��Center=�6:   .+�5�      �� $��Registros : ��6$   
�������$
��Bold"�6K   $��5 0 0 0q�6]   �� 5�      -   5        $��Tx_registros��5�   $   
�������68   5�      �� 5�      $. 
Horizontal=�6!   $��Center=�6:   $��Right=�6U   $�� 0 0 5 0q�6q   .+�5�      �� -   5        $��
BtnReporte��5�   !   $��Reporte��63   $ 1��6_   $��5��6g   / � Cursor$
/ Hand��6w   #��[  6�   $	��100��6�   �� 5�      -	   5        $��Exportar Xls��5�   !   $ 1��6N   $��5��6V   $
/ Hand��6f   #��[  6t   $	��100��6�   5�      5�      5�      5�      5�      5�      