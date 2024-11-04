import * as React from "react";
import ToggleButton from "@mui/material/ToggleButton";
import ToggleButtonGroup from "@mui/material/ToggleButtonGroup";
import { TbChartCandle } from "react-icons/tb";
import { GoGraph } from "react-icons/go";

interface ToggleButtonNotEmptyProps {
  candleSelected: boolean;
  setCandleSelected: (value: boolean) => void;
}

export default function ToggleButtonNotEmpty({
  candleSelected,
  setCandleSelected,
}: ToggleButtonNotEmptyProps) {
  const handleAlignment = (
    event: React.MouseEvent<HTMLElement>,
    newAlignment: string | null
  ) => {
    if (newAlignment) {
      setCandleSelected(newAlignment === "right");
    }
  };

  return (
    <ToggleButtonGroup
      value={candleSelected ? "right" : "left"}
      exclusive
      onChange={handleAlignment}
      aria-label="chart type"
    >
      <ToggleButton value="left" aria-label="line chart">
        <GoGraph />
      </ToggleButton>
      <ToggleButton value="right" aria-label="candlestick chart">
        <TbChartCandle />
      </ToggleButton>
    </ToggleButtonGroup>
  );
}
