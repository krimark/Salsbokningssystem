/*
Copyright Dinamenta, UAB. http://www.dhtmlx.com
To use this component please contact sales@dhtmlx.com to obtain license
*/
Scheduler.plugin(function(a){a.__recurring_template='<div class="dhx_form_repeat"> <form> <div class="dhx_repeat_left"> <label><input class="dhx_repeat_radio" type="radio" name="repeat" value="day" />Quotidienne</label><br /> <label><input class="dhx_repeat_radio" type="radio" name="repeat" value="week"/>Hebdomadaire</label><br /> <label><input class="dhx_repeat_radio" type="radio" name="repeat" value="month" checked />Mensuelle</label><br /> <label><input class="dhx_repeat_radio" type="radio" name="repeat" value="year" />Annuelle</label> </div> <div class="dhx_repeat_divider"></div> <div class="dhx_repeat_center"> <div style="display:none;" id="dhx_repeat_day"> <label><input class="dhx_repeat_radio" type="radio" name="day_type" value="d"/>Chaque</label><input class="dhx_repeat_text" type="text" name="day_count" value="1" />jour<br /> <label><input class="dhx_repeat_radio" type="radio" name="day_type" checked value="w"/>Chaque journ\u00e9e de travail</label> </div> <div style="display:none;" id="dhx_repeat_week"> R\u00e9p\u00e9ter toutes les<input class="dhx_repeat_text" type="text" name="week_count" value="1" />semaine:<br /> <table class="dhx_repeat_days"> <tr> <td> <label><input class="dhx_repeat_checkbox" type="checkbox" name="week_day" value="1" />Lundi</label><br /> <label><input class="dhx_repeat_checkbox" type="checkbox" name="week_day" value="4" />Jeudi</label> </td> <td> <label><input class="dhx_repeat_checkbox" type="checkbox" name="week_day" value="2" />Mardi</label><br /> <label><input class="dhx_repeat_checkbox" type="checkbox" name="week_day" value="5" />Vendredi</label> </td> <td> <label><input class="dhx_repeat_checkbox" type="checkbox" name="week_day" value="3" />Mercredi</label><br /> <label><input class="dhx_repeat_checkbox" type="checkbox" name="week_day" value="6" />Samedi</label> </td> <td> <label><input class="dhx_repeat_checkbox" type="checkbox" name="week_day" value="0" />Dimanche</label><br /><br /> </td> </tr> </table> </div> <div id="dhx_repeat_month"> <label><input class="dhx_repeat_radio" type="radio" name="month_type" value="d"/>R\u00e9p\u00e9ter</label><input class="dhx_repeat_text" type="text" name="month_day" value="1" />jour chaque<input class="dhx_repeat_text" type="text" name="month_count" value="1" />mois<br /> <label><input class="dhx_repeat_radio" type="radio" name="month_type" checked value="w"/>Le</label><input class="dhx_repeat_text" type="text" name="month_week2" value="1" /><select name="month_day2"><option value="1" selected >Lundi<option value="2">Mardi<option value="3">Mercredi<option value="4">Jeudi<option value="5">Vendredi<option value="6">Samedi<option value="0">Dimanche</select>chaque<input class="dhx_repeat_text" type="text" name="month_count2" value="1" />mois<br /> </div> <div style="display:none;" id="dhx_repeat_year"> <label><input class="dhx_repeat_radio" type="radio" name="year_type" value="d"/>Chaque</label><input class="dhx_repeat_text" type="text" name="year_day" value="1" />jour<select name="year_month"><option value="0" selected >Janvier<option value="1">F\u00e9vrier<option value="2">Mars<option value="3">Avril<option value="4">Mai<option value="5">Juin<option value="6">Juillet<option value="7">Ao\u00fbt<option value="8">Septembre<option value="9">Octobre<option value="10">Novembre<option value="11">D\u00e9cembre</select>mois<br /> <label><input class="dhx_repeat_radio" type="radio" name="year_type" checked value="w"/>Le</label><input class="dhx_repeat_text" type="text" name="year_week2" value="1" /><select name="year_day2"><option value="1" selected >Lundi<option value="2">Mardi<option value="3">Mercredi<option value="4">Jeudi<option value="5">Vendredi<option value="6">Samedi<option value="0">Dimanche</select>du<select name="year_month2"><option value="0" selected >Janvier<option value="1">F\u00e9vrier<option value="2">Mars<option value="3">Avril<option value="4">Mai<option value="5">Juin<option value="6">Juillet<option value="7">Ao\u00fbt<option value="8">Septembre<option value="9">Octobre<option value="10">Novembre<option value="11">D\u00e9cembre</select><br /> </div> </div> <div class="dhx_repeat_divider"></div> <div class="dhx_repeat_right"> <label><input class="dhx_repeat_radio" type="radio" name="end" checked/>Pas de date d&quot;ach\u00e8vement</label><br /> <label><input class="dhx_repeat_radio" type="radio" name="end" />Apr\u00e8s</label><input class="dhx_repeat_text" type="text" name="occurences_count" value="1" />occurrences<br /> <label><input class="dhx_repeat_radio" type="radio" name="end" />Fin</label><input class="dhx_repeat_date" type="text" name="date_of_end" value="'+
a.config.repeat_date_of_end+'" /><br /> </div> </form> </div> <div style="clear:both"> </div>'});