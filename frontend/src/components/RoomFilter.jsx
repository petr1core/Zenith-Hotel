import React, { useState } from 'react';
import { Box, FormControl, InputLabel, Select, MenuItem, TextField, Button, Grid } from '@mui/material';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { ru } from 'date-fns/locale';

const RoomFilter = ({ onFilter }) => {
    const [filters, setFilters] = useState({
        minPrice: '',
        maxPrice: '',
        roomType: 'all',
        checkInDate: null,
        checkOutDate: null,
        minCapacity: '',
        maxCapacity: '',
        floor: '',
    });

    const fieldStyles = {
        '& label.Mui-focused': {
            color: '#4f46e5',
        },
        '& .MuiInputLabel-root': {
            color: '#4f46e5',
        },
        '& .MuiOutlinedInput-root': {
            '& .MuiInputBase-input': {
                color: '#e6e6e6', // Цвет вводимого текста
            },
            '& fieldset': {
                borderColor: '#4f46e5',
            },
            '&:hover fieldset': {
                borderColor: '#4f46e5',
            },
            '&.Mui-focused fieldset': {
                borderColor: '#4f46e5',
            },
        },
        '& .MuiSelect-icon': {
            color: '#4f46e5',
        },
    };

    const numberFieldStyles = {
        ...fieldStyles,
        // Скрываем стрелки в числовых полях
        '& input[type=number]': {
            '-moz-appearance': 'textfield',
        },
        '& input[type=number]::-webkit-outer-spin-button, & input[type=number]::-webkit-inner-spin-button': {
            '-webkit-appearance': 'none',
            margin: 0,
        },
    };

    const handleChange = (event) => {
        const { name, value } = event.target;
        setFilters((prev) => ({
            ...prev,
            [name]: value,
        }));
    };

    const handleDateChange = (name, value) => {
        setFilters((prev) => ({
            ...prev,
            [name]: value,
        }));
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        onFilter(filters);
    };

    return (
        <Box component="form" onSubmit={handleSubmit} sx={{ mb: 4, p: 2, bgcolor: '#16213e', borderRadius: 2 }}>
            <Grid container spacing={2} alignItems="center">
                <Grid item xs={12} sm={6} md={3}>
                    <FormControl fullWidth sx={fieldStyles}>
                        <InputLabel id="room-type-label">Тип номера</InputLabel>
                        <Select
                            labelId="room-type-label"
                            name="roomType"
                            value={filters.roomType}
                            label="Тип номера"
                            onChange={handleChange}
                        >
                            <MenuItem value="all">Все типы</MenuItem>
                            <MenuItem value="Standard">Стандарт</MenuItem>
                            <MenuItem value="Superior">Стандарт плюс</MenuItem>
                            <MenuItem value="FamilyRoom">Семейный</MenuItem>
                            <MenuItem value="Deluxe">Делюкс</MenuItem>
                            <MenuItem value="Suite">Люкс</MenuItem>
                        </Select>
                    </FormControl>
                </Grid>

                <Grid item xs={12} sm={6} md={3}>
                    <TextField
                        fullWidth
                        label="Минимальная цена"
                        name="minPrice"
                        type="number"
                        value={filters.minPrice}
                        onChange={handleChange}
                        InputProps={{ inputProps: { min: 0 } }}
                        sx={numberFieldStyles}
                    />
                </Grid>

                <Grid item xs={12} sm={6} md={3}>
                    <TextField
                        fullWidth
                        label="Максимальная цена"
                        name="maxPrice"
                        type="number"
                        value={filters.maxPrice}
                        onChange={handleChange}
                        InputProps={{ inputProps: { min: 0 } }}
                        sx={numberFieldStyles}
                    />
                </Grid>

                <Grid item xs={12} sm={6} md={3}>
                    <TextField
                        fullWidth
                        label="Этаж"
                        name="floor"
                        type="number"
                        value={filters.floor}
                        onChange={handleChange}
                        InputProps={{ inputProps: { min: 1 } }}
                        sx={numberFieldStyles}
                    />
                </Grid>

                <Grid item xs={12} sm={6} md={3}>
                    <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={ru}>
                        <DatePicker
                            label="Дата заезда"
                            value={filters.checkInDate}
                            onChange={(newValue) => handleDateChange('checkInDate', newValue)}
                            minDate={new Date()}
                            sx={{ ...fieldStyles, width: '100%' }}
                        />
                    </LocalizationProvider>
                </Grid>

                <Grid item xs={12} sm={6} md={3}>
                    <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={ru}>
                        <DatePicker
                            label="Дата выезда"
                            value={filters.checkOutDate}
                            onChange={(newValue) => handleDateChange('checkOutDate', newValue)}
                            minDate={filters.checkInDate || new Date()}
                            sx={{ ...fieldStyles, width: '100%' }}
                        />
                    </LocalizationProvider>
                </Grid>

                <Grid item xs={12} sm={6} md={3}>
                    <TextField
                        fullWidth
                        label="Минимальная вместимость"
                        name="minCapacity"
                        type="number"
                        value={filters.minCapacity}
                        onChange={handleChange}
                        InputProps={{ inputProps: { min: 1 } }}
                        sx={numberFieldStyles}
                    />
                </Grid>

                <Grid item xs={12} sm={6} md={3}>
                    <TextField
                        fullWidth
                        label="Макс. вместимость"
                        name="maxCapacity"
                        type="number"
                        value={filters.maxCapacity}
                        onChange={handleChange}
                        InputProps={{ inputProps: { min: 1 } }}
                        sx={numberFieldStyles}
                    />
                </Grid>

                <Grid item xs={12} md={12}>
                    <Button
                        type="submit"
                        variant="contained"
                        fullWidth
                        sx={{
                            height: '56px',
                            bgcolor: '#0f3460',
                            '&:hover': {
                                bgcolor: '#1a4b8c',
                            },
                        }}
                    >
                        Применить фильтры
                    </Button>
                </Grid>
            </Grid>
        </Box>
    );
};

export default RoomFilter;
