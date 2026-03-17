<?php

ini_set('display_errors', 'On');
ini_set('html_errors', 0);


class CalmarMooringCalculation
{
	const DensiteEau = 1.025;
    const DensiteAir = 0.00129;
    const DensiteMetal = 7.85;
    const DensiteLest = 7.32;

    const COEF_TRAINEE_VENT = 1.2;
    const COEF_TRAINEE_COURANT = 1.2;
    const COEF_TRAINEE_CHAINE = 1.2;

    const TRAINEE_COEF = 1.2;

    const SeaBedInternalFrictionAngle = 45;
    const CoefficientSecuriteMasseCorpsMort = 1.5;

	public $buoy;
	public $chain;
	public $chain_quality;
	
	public $mass_equipment;
	public $density_sinker;
	
	public $depth;
	public $tide;
	public $wave;
	public $wave_period;
	public $wind_speed;
	public $current_speed;

	public $length_catenary;

	public function wave_max()
    {
    	return round($this->wave * 1.85,2,PHP_ROUND_HALF_DOWN);
    }
	public function wave_speed()
    {
    	return pi() * $this->wave / $this->wave_period;
    }
	public function depth_max()
    {
    	return $this->depth + $this->tide + ($this->wave_max() / 2);
    }
	public function depth_min()
    {
    	return $this->depth - ($this->wave_max() / 2);
    }
	public function height_catenary()
    {
    	return $this->depth_max() + $this->buoy->depth_below_mooring_point();
    }
	public function speed_current_surface()
    {
    	return $this->current_speed + ($this->wind_speed * 0.015) + $this->wave_speed();
    }
	public function chain_diameter()
    {
    	return $this->chain->dn / 1000;
    }
	public function chain_area()
    {
    	return $this->height_catenary() * ($this->chain_diameter() * 2.65);
    }
	public function weight_lineic()
    {
    	return $this->chain->mass_lineic / self::DensiteMetal * (self::DensiteMetal - self::DensiteEau);
    }
	public function weight_catenary()
    {
    	return $this->length_catenary * $this->weight_lineic();
    }
	public function weight_ballast()
    {
    	return $this->buoy->mass_ballast / self::DensiteMetal * (self::DensiteMetal - self::DensiteEau);
    }
	public function total_displacement()
    {
    	return $this->buoy->mass() + $this->weight_ballast() + $this->weight_catenary();
    }
	public function total_volume_displacement()
    {
    	return $this->total_displacement() / self::DensiteEau;
    }
	public function wind_force()
    {
    	return (0.5 * ($this->wind_speed ** 2) *  self::DensiteAir * (self::COEF_TRAINEE_VENT * $this->buoy->surface_emerged()) / 9.81);
    }
	public function current_force_chain()
    {
    	return (0.5 * ($this->current_speed ** 2) * self::DensiteEau * (self::COEF_TRAINEE_CHAINE * $this->chain_area())) / 9.81;
    }
	public function current_force_buoy()
    {
    	return (0.5 * ($this->speed_current_surface() ** 2) *  self::DensiteEau * (self::COEF_TRAINEE_COURANT * $this->buoy->surface_draft()) / 9.81);
    }
	public function horizontal_force()
    {
    	//echo "<br> h force:".$this->wind_force()."-".$this->current_force_chain()."-".$this->current_force_buoy()."<br>";
    	return $this->wind_force() + $this->current_force_chain() + $this->current_force_buoy();
    }
	public function length_catenary_update()
    {
    	$this->length_catenary = sqrt($this->height_catenary() ** 2 + (2 * (($this->horizontal_force() * 1000) / $this->weight_lineic()) * $this->height_catenary()));
    	return $this->length_catenary;
    }
	public function tension_chain()
    {
    	return sqrt(($this->weight_catenary() / 1000) ** 2 + $this->horizontal_force() ** 2);
    }
	public function angle_tangence()
    {
    	return rad2deg(acos($this->weight_catenary()/($this->tension_chain()*1000)));
    }
	public function security_coefficient_chain()
    {
    	return $this->chain->charge_at_quality($this->chain_quality) / ((($this->height_catenary() * $this->weight_lineic()) / 1000) + $this->horizontal_force());
    }
	public function floatability_left()
    {
    	return $this->buoy->floatability_left();
    }
	public function swing_radius()
    {
    	return (($this->horizontal_force() * 1000) / $this->weight_lineic()) * acosh(($this->height_catenary() / (($this->horizontal_force() * 1000) / $this->weight_lineic())) + 1);
    }
	public function sinker_mass_min()
    {
    	return self::CoefficientSecuriteMasseCorpsMort * $this->horizontal_force() * ($this->density_sinker / ($this->density_sinker - self::DensiteEau)) / (tan(deg2rad(self::SeaBedInternalFrictionAngle)));
    }
	public function sinker_weight_water()
    {
    	return $this->sinker_mass_min() / ($this->density_sinker * ($this->density_sinker - self::DensiteEau));
    }
}

class Calmar_chain
{
	public $dn;
	public $type;
	public $mass_lineic;

	public $quality_ce_1;
	public $quality_ce_2;
	public $quality_ce_3;

	public function charge_at_quality($quality = 2)
	{
		switch ($quality) {
    		case 1:
        		return $this->quality_ce_1;
    		case 2:
        		return $this->quality_ce_2;
    		case 3:
        		return $this->quality_ce_3;
		}
	}
}

class Calmar_buoy
{	
	public $structure;
	public $float;
	public $pylon;
	public $equipment;

	public $mass_ballast;

	public function mass()
    {
    	$mass_t = $this->mass_equipment() + $this->mass_pylon();
    	
    	if ($this->structure) 
        	$mass_t += $this->structure->mass;
    	if ($this->float) 
        	$mass_t += $this->float->mass;
    
    	return $mass_t;
    }

	public function mass_pylon()
    {
    	$mass_p = 0;
    	foreach ($this->pylon as $pyl)
        	$mass_p += $pyl->mass;
		return $mass_p;
    }

	public function mass_equipment()
    {
    	$mass_e = 0;
    	foreach ($this->equipment as $equ)
        	$mass_e += $equ->mass;
		return $mass_e;
    }

	public function surface_pylon_equipment()
    {
    	$surface_t = 0;
    	foreach ($this->pylon as $pyl)
        	$surface_t += $pyl->elements->surface();
    	foreach ($this->equipment as $equ)
        	$surface_t += $equ->elements->surface();
		return $surface_t;
    }

	public function init()
	{
    	// Init des immersions 
        $this->structure->float = $this->float;

        $this->float->set_draft(0);
        $this->structure->set_draft($this->float->draft());    	
	}
	public function set_draft($height)
	{
        $this->structure->float = $this->float;

        $this->float->set_draft($height);
        $this->structure->set_draft($this->float->draft()); 
    	
	}
	public function depth_below_mooring_point()
	{
		return -1 * ($this->float->draft() + $this->structure->offset_mooring_point);
	}
	public function surface_draft()
    {
    	return $this->float->surface_draft() + $this->structure->surface_draft();
    }
	public function surface_emerged()
	{
		return $this->float->surface_emerged() + $this->surface_pylon_equipment();
	}
	public function volume()
    {
    	return $this->float->volume() + $this->structure->volume();
    }
	public function volume_draft()
    {
    	return $this->float->volume_draft() + $this->structure->volume_draft();
    }
	public function floatability_left()
	{
		return round((($this->volume() - (($this->float->volume_draft() + $this->structure->volume_draft()) / 1000)) / $this->volume()) * 100,2);
	}
	public function freeboard()
    {
    	return $this->float->height_max() - $this->float->draft();
    }
	public function draft()
    {
    	return $this->float->draft() + $this->structure->offset_float;
    }
}

class Calmar_structure
{
	public $mass;
	
	public $offset_float;
	public $offset_mooring_point;
	public $float;

	public $elements;
		
	private $height_water;
	private $surface_draft;
	private $surface_emerged;
	private $volume_draft;

	public function height_water_float()
	{
    	return $this->offset_float + $this->height_water;
	}
	public function height_end_float()
	{
    	return $this->offset_float + $this->float->height_max();
	}
	public function set_draft($height)
	{
    	$this->height_water = $height;

    	$height_low = 0;
    	$this->surface_draft = 0;
    	$this->surface_emerged = 0;
    	$this->volume_draft = 0;
    
    	foreach ($this->elements as $elem)
        {
            if ($height_low + $elem->height_total <= $this->height_water_float())
            {
            	$this->volume_draft += $elem->volume();
            
            	if (!($height_low >= $this->offset_float && $height_low < $this->height_end_float()))
                	$this->surface_draft += $elem->surface();
            }
            if (($this->height_water_float() > $height_low) && (($height_low + $elem->height_total) > $this->height_water_float()))
            {
            	// calcul du volume pour la hauteur donnée
            	$this->volume_draft += $elem->volume_at_height($this->height_water_float() - $height_low);
            
            	if ($height_low < $this->offset_float && $this->height_water_float() > $height_low)
                {
            		$arr = $elem->surface_at_height($this->height_water_float() - $height_low);
            		$this->surface_draft += $arr[0];
                }
            	if ($this->height_water_float() > $this->height_end_float())
                {
            		$arr = $elem->surface_at_height($this->height_water_float() - $this->height_end_float());
            		$this->surface_draft += $arr[0];
                }
            	if ((($height_low + $elem->height_total) > $this->height_end_float()) && (($height_low + $elem->height_total) > $this->height_water_float()))
                {
            		$arr = $elem->surface_at_height(($height_low + $elem->height_total) - $this->height_water_float());
            		$this->surface_emerged += $arr[1];
                }
            }
            if ($height_low >= $this->height_water_float())
            {
            	if (!(($height_low >= $this->offset_float) && ($height_low < $this->height_end_float())))
            		$this->surface_emerged += $elem->surface();
            }
        	
        	$height_low+=$elem->height_total;
        }
            
        // Multiplication par 1000 pour avoir des dcm3
    	$this->volume_draft = $this->volume_draft * 1000;
    
	}
	public function draft()
	{
    	return $this->height_water;
	}
	public function surface_draft()
    {
    	return $this->surface_draft;
    }
	public function surface_emerged()
	{
		return $this->surface_emerged;
	}
	public function volume_draft()
    {
    	return $this->volume_draft;
    }
	public function volume()
	{
    	$volume_total = 0;
    	foreach ($this->elements as $elem)
        	$volume_total += $elem->volume();
		return $volume_total;
	}
	public function height_max()
    {
    	$h_max = 0;
    	foreach ($this->elements as $elem)
        	$h_max += $elem->height_total;
		return $h_max;
    }
}

class Calmar_float
{
	public $mass;
	public $elements;

	private $height_water;
	private $surface_draft;
	private $surface_emerged;
	private $volume_draft;

	public function set_draft($height)
	{
    	$this->height_water = $height;
    
    	$height_low = 0;
    	$this->surface_draft = 0;
    	$this->surface_emerged = 0;
    	$this->volume_draft = 0;
    
    	foreach ($this->elements as $elem)
        {
            if ($height_low+$elem->height_total <= $this->height_water)
            {
            	$this->volume_draft += $elem->volume();
            	$this->surface_draft += $elem->surface();
            }
            if (($this->height_water > $height_low) && ($elem->height_total > $this->height_water))
            {
            	// calcul du volume pour la hauteur donnée
            	$this->volume_draft += $elem->volume_at_height($this->height_water-$height_low);
            	// Calcule de la surface immergée et emmergée
            	$arr = $elem->surface_at_height($this->height_water-$height_low);
            	$this->surface_draft += $arr[0];
            	$this->surface_emerged += $arr[1];
            }
            if ($height_low >= $this->height_water)
            {
            	$this->surface_emerged += $elem->surface();
            }
        	
        	$height_low+=$elem->height_total;
        }
    
        // Multiplication par 1000 pour avoir des dcm3
    	$this->volume_draft = $this->volume_draft * 1000;
	}
	public function draft()
	{
    	return $this->height_water;
	}
	public function surface_draft()
    {
    	return $this->surface_draft;
    }
	public function surface_emerged()
	{
		return $this->surface_emerged;
	}
	public function volume_draft()
    {
    	return $this->volume_draft;
    }
	public function volume()
	{
    	$volume_total = 0;
    	foreach ($this->elements as $elem)
        	$volume_total += $elem->volume();
		return $volume_total;
	}
	public function height_max()
    {
    	$h_max = 0;
    	foreach ($this->elements as $elem)
        	$h_max += $elem->height_total;
		return $h_max;
    }
}

class Calmar_pylone
{
	public $mass;
	public $elements;
}

class Calmar_equipment
{
	public $mass;
	public $elements;
}


class Calmar_elements_standard
{
    public $length_low;
    public $length_high;
    public $height_total;

	public function surface()
    {
    	return $this->CalculTrapeze($this->length_low,$this->length_high,$this->height_total);
    }

	public function surface_at_height($height)
    {
        $L_Inter = $this->length_low + ($height * ($this->length_high - $this->length_low) / $this->height_total);
        return array($this->CalculTrapeze($this->length_low, $L_Inter, $height), $this->CalculTrapeze($L_Inter, $this->length_high, $this->height_total - $height));
    }
	public function CalculTrapeze($length1,$length2,$height)
    {
    	return ($height * ($length1 + $length2) / 2);
    }
}

class Calmar_elements_tronc_cone extends Calmar_elements_standard
{
    public $length_inter;
	public $volume_real;

	public function volume()
    {
    	if ($this->volume_real == 0) return $this->calcul_volume();
    	return $this->volume_real;
    }
	public function diameter_surface_low()
    {
    	return pi() * ($this->length_low / 2) ** 2;
    }
	public function diameter_surface_high()
    {
    	return pi() * ($this->length_high / 2) ** 2;
    }
	public function diameter_surface_inter()
    {
    	return pi() * ($this->length_inter / 2) ** 2;
    }
	public function ratio_volume()
    {
    	if ($this->calcul_volume() == 0) return 1;
    	return $this->volume() / $this->calcul_volume();
    }
	public function calcul_volume()
    {
    	$vol_total = $this->calcul_volume_truncated_cone($this->height_total,$this->diameter_surface_low(),$this->diameter_surface_high());
    	$vol_total_inter = $this->calcul_volume_truncated_cone($this->height_total,$this->diameter_surface_inter(),$this->diameter_surface_inter());
    	return $vol_total - $vol_total_inter;
    }
	public function volume_at_height($height)
    {
    	$len_inter = $this->length_low + ($height * ($this->length_high - $this->length_low) / $this->height_total);
		$surface_len_inter = pi() * ($len_inter / 2) ** 2;
    
    	$vol_h_total = $this->calcul_volume_truncated_cone($height,$this->diameter_surface_low(),$surface_len_inter);
    	$vol_h_total_inter = $this->calcul_volume_truncated_cone($height,$this->diameter_surface_inter(),$this->diameter_surface_inter());
    
    	return ($vol_h_total - $vol_h_total_inter) * $this->ratio_volume();
    }
	public function calcul_volume_truncated_cone($height,$surface1,$surface2)
    {
    	return $height / 3 * ($surface1 + sqrt($surface1 * $surface2) + $surface2);
    }

}

function array_elements_tronc_cone($elements)
{
	$arr = array();

	foreach ($elements as $elem_item)
    {
    	if (isset($elem_item["attributes"]))
            $elem = $elem_item["attributes"];
        else
            $elem = $elem_item;
    
    	if ($elem)
        {
    		$new_element = new Calmar_elements_tronc_cone;
    		$new_element->length_low = $elem["D0"];
    		$new_element->length_high = $elem["D1"];
    		$new_element->length_inter = $elem["DI"];
    		$new_element->height_total = $elem["H"];
    		$new_element->volume_real = $elem["Volume"];
    		$arr[] = $new_element;
        }
    }

	return $arr;
}

/**
 * @param SimpleXMLElement $xml
 * @return array
 */
function xmlToArray(SimpleXMLElement $xml): array
{
    $parser = function (SimpleXMLElement $xml, array $collection = []) use (&$parser) {
        $nodes = $xml->children();
        $attributes = $xml->attributes();

        if (0 !== count($attributes)) {
            foreach ($attributes as $attrName => $attrValue) {
                $collection['attributes'][$attrName] = strval($attrValue);
            }
        }

        if (0 === $nodes->count()) {
            $collection['value'] = strval($xml);
            return $collection;
        }

        foreach ($nodes as $nodeName => $nodeValue) {
            if (count($nodeValue->xpath('../' . $nodeName)) < 2) {
                $collection[$nodeName] = $parser($nodeValue);
                continue;
            }

            $collection[$nodeName][] = $parser($nodeValue);
        }

        return $collection;
    };

    return [
        $xml->getName() => $parser($xml)
    ];
}

function parse_xml($xml)
{
	$xml_array = xmlToArray($xml);

	$b = new Calmar_buoy;
	$b->equipment = array();
	$b->pylon = array();
	$b->float = new Calmar_float;
	$b->structure = new Calmar_structure;
	$b->structure->float = $b->float;

	$b->structure->mass = $xml_array["Calmar"]["Buoy"]["Structure"]["attributes"]["Masse"];
	$b->structure->offset_float = $xml_array["Calmar"]["Buoy"]["Structure"]["attributes"]["OffsetFlotteur"];
	$b->structure->offset_mooring_point = $xml_array["Calmar"]["Buoy"]["Structure"]["attributes"]["OffsetOrganeau"];
	$b->structure->elements = array_elements_tronc_cone($xml_array["Calmar"]["Buoy"]["Structure"]["ElementDimItem"]);

	$b->float->mass = $xml_array["Calmar"]["Buoy"]["Flotteur"]["attributes"]["Masse"];
	$b->float->elements = array_elements_tronc_cone($xml_array["Calmar"]["Buoy"]["Flotteur"]["ElementDimItem"]);

	foreach ($xml_array["Calmar"]["Buoy"]["Pylone"] as $pyl_item)
    {
    	if (isset($pyl_item["attributes"]))
            $pylone = $pyl_item["attributes"];
        else
            $pylone = $pyl_item;
            
    	if ($pylone)
        {
    		$new_pylone = new Calmar_pylone;
    		$new_pylone->mass = $pylone["Masse"];
    		$new_pylone->elements = new Calmar_elements_standard;
    		$new_pylone->elements->length_low = $pylone["WidthLow"];
    		$new_pylone->elements->length_high = $pylone["WidthHigh"];
    		$new_pylone->elements->height_total = $pylone["Height"];
        	$b->pylon[] = $new_pylone;
        }
    }

	foreach ($xml_array["Calmar"]["Buoy"]["Equipement"] as $equ_item)
    {
    	if (isset($equ_item["attributes"]))
            $equipment = $equ_item["attributes"];
        else
            $equipment = $equ_item;
            
    	if ($equipment)
        {
    		$new_equipment = new Calmar_equipment;
    		$new_equipment->mass = $equipment["Masse"];
    		$new_equipment->elements = new Calmar_elements_standard;
    		$new_equipment->elements->length_low = $equipment["WidthLow"];
    		$new_equipment->elements->length_high = $equipment["WidthHigh"];
    		$new_equipment->elements->height_total = $equipment["Height"];
        	$b->equipment[] = $new_equipment;
        }
    }

	return $b;
}


$json = file_get_contents('php://input');
$data_json = json_decode($json);

if (!isset($data_json->model))
	$xml = simplexml_load_file("BC1242.xmlMB");
else
	$xml = simplexml_load_string($data_json->model, 'SimpleXMLElement', LIBXML_NOCDATA);

$process_mooring_calculation = new CalmarMooringCalculation();
$process_mooring_calculation->buoy = parse_xml($xml);

$process_mooring_calculation->chain = new Calmar_chain();
$process_mooring_calculation->chain->dn = $data_json->chaine->dn;
$process_mooring_calculation->chain->type = $data_json->chaine->type;
$process_mooring_calculation->chain->mass_lineic = $data_json->chaine->masse_lineique;
$process_mooring_calculation->chain->quality_ce_1 = $data_json->chaine->charge_epreuve_ql;
$process_mooring_calculation->chain->quality_ce_2 = $data_json->chaine->charge_epreuve_ql;
$process_mooring_calculation->chain->quality_ce_3= $data_json->chaine->charge_epreuve_ql;
$process_mooring_calculation->chain_quality = 2;

$process_mooring_calculation->current_speed = $data_json->vitesse_courant;
$process_mooring_calculation->density_sinker = $data_json->densite_cm;
$process_mooring_calculation->depth = $data_json->profondeur;
$process_mooring_calculation->tide = $data_json->marnage;
$process_mooring_calculation->wave = $data_json->houle_significant;
$process_mooring_calculation->wave_period = $data_json->periode_houle;
$process_mooring_calculation->wind_speed = $data_json->vitesse_vent;

$process_mooring_calculation->buoy->init();
$process_mooring_calculation->length_catenary = $process_mooring_calculation->height_catenary();
$i = 0;
do
{
	$process_mooring_calculation->buoy->set_draft($i);
	$process_mooring_calculation->length_catenary_update();

	$i+=0.005;
} while ($process_mooring_calculation->buoy->volume_draft() < $process_mooring_calculation->total_volume_displacement());

$Resultat = array();
$Resultat["LongueurCatenaire"] = $process_mooring_calculation->length_catenary;
$Resultat["TensionMaxMouillage"] = $process_mooring_calculation->tension_chain();
$Resultat["MasseCM"] = $process_mooring_calculation->sinker_mass_min();
$Resultat["Floatibilite"] = $process_mooring_calculation->floatability_left();
$Resultat["FrancBord"] = $process_mooring_calculation->buoy->freeboard();
$Resultat["SurfaceLatImmerge"] = $process_mooring_calculation->buoy->surface_draft();
$Resultat["RayonEvitage"] = $process_mooring_calculation->swing_radius();
$Resultat["CoeficientSecuriteChaine"] = $process_mooring_calculation->security_coefficient_chain();
$Resultat["TraineHorizontale"] = $process_mooring_calculation->horizontal_force();
$Resultat["DeplacementBouee"] = $process_mooring_calculation->total_displacement()/1000;
$Resultat["TirantEau"] = $process_mooring_calculation->buoy->draft();
$Resultat["SurfaceLatEmerge"] = $process_mooring_calculation->buoy->surface_emerged();

echo json_encode($Resultat);

?>